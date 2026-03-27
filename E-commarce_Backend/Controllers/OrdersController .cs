using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos.Orders;
using E_commarce_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_commarce_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public OrdersController(ECommerceDbContext context)
        {
            _context = context;
        }

        // 🔑 Get UserId from JWT claim safely
        private string GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                         ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID claim is missing");

            return userId;
        }

        // 🛒 Create an order from cart
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
        {
            var userId = GetUserId();

            // Get all cart items for the current user
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Include(c => c.Cart)
                .Where(c => c.Cart.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest("Cart is empty");

            decimal total = 0;
            var stockIssues = new List<string>();

            // Check stock
            foreach (var item in cartItems)
            {
                var product = item.Product;
                if (product.Stock < item.Quantity)
                    stockIssues.Add($"Not enough stock for {product.Name}. Available: {product.Stock}");

                total += product.Price * item.Quantity;
            }

            if (stockIssues.Any())
                return BadRequest(string.Join(", ", stockIssues));

            // Create order
            var order = new Order
            {
                UserId = userId, // string
                TotalPrice = total,
                ShippingAddress = dto.ShippingAddress,
                Phone = dto.Phone,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Create order items and update stock
            foreach (var item in cartItems)
            {
                item.Product.Stock -= item.Quantity;

                _context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = item.Product.Price
                });
            }

            // Clear cart
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return Ok(new { orderId = order.Id, message = "Order created successfully" });
        }

        // 🧾 Get orders of current user
        [HttpGet("my")]
        public async Task<IActionResult> MyOrders()
        {
            var userId = GetUserId();

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new
                {
                    o.Id,
                    o.TotalPrice,
                    o.Status,
                    o.CreatedAt,
                    o.ShippingAddress,
                    o.Phone,
                    ItemCount = o.Items.Count
                })
                .ToListAsync();

            return Ok(orders);
        }

        // 📦 Get details of a specific order
        [HttpGet("{id}")]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var userId = GetUserId();

            var order = await _context.Orders
           .Include(o => o.Items)
           .ThenInclude(i => i.Product) // Ensure Product is included
           .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return NotFound("Order not found");

            var items = order.Items.Select(i => new
            {
                i.Id,
                i.ProductId,
                i.Quantity,
                i.PriceAtPurchase,
                Subtotal = i.Quantity * i.PriceAtPurchase,
                ProductName = i.Product?.Name // Safely access Product.Name
            }).ToList();

            return Ok(new { Order = order, Items = items, Total = order.TotalPrice });

        }
    }
}