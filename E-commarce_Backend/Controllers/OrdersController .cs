using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos;
using E_commarce_Backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_commarce_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ECommerceDbContext _context;

        public OrdersController(ECommerceDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
        {
            var userId = int.Parse(User.FindFirst("id").Value);

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.Id == userId)
                .ToListAsync();

            if (!cartItems.Any())
                return BadRequest("Cart is empty");

            decimal total = 0;
            var stockIssues = new List<string>();

            // Check stock for all items first
            foreach (var item in cartItems)
            {
                var product = item.Product;

                if (product.Stock < item.Quantity)
                {
                    stockIssues.Add($"Not enough stock for {product.Name}. Available: {product.Stock}");
                }

                total += product.Price * item.Quantity;
            }

            if (stockIssues.Any())
                return BadRequest(string.Join(", ", stockIssues));

            // Create order
            var order = new Order
            {
                UserId = userId,
                TotalPrice = total,
                ShippingAddress = dto.ShippingAddress,
                Phone = dto.Phone,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Process each cart item
            foreach (var item in cartItems)
            {
                // Update stock
                item.Product.Stock -= item.Quantity;

                // Create order item
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

            return Ok(new
            {
                orderId = order.Id,
                message = "Order created successfully"
            });
        }

        [HttpGet("my")]
        [Authorize]
        public async Task<IActionResult> MyOrders()
        {
            var userId = int.Parse(User.FindFirst("id").Value);

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Select(o => new
                {
                    o.Id,
                    o.TotalPrice,
                    o.Status,
                    o.CreatedAt,
                    o.ShippingAddress,
                    o.Phone,
                    ItemCount = _context.OrderItems.Count(i => i.OrderId == o.Id)
                })
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var userId = int.Parse(User.FindFirst("id").Value);

            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return NotFound("Order not found");

            var items = await _context.OrderItems
                .Where(i => i.OrderId == id)
                .Select(i => new
                {
                    i.Id,
                    i.ProductId,
                    i.Quantity,
                    i.PriceAtPurchase,
                    Subtotal = i.Quantity * i.PriceAtPurchase,
                    ProductName = _context.Products
                        .Where(p => p.Id == i.ProductId)
                        .Select(p => p.Name)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(new
            {
                Order = order,
                Items = items,
                Total = order.TotalPrice
            });
        }
    }
}