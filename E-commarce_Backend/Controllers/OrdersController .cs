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
    public class OrdersController(ECommerceDbContext context) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
        {
            var userIdClaim = User.FindFirst("id");
            if (userIdClaim == null)
                return Unauthorized("User ID claim is missing.");
            var userId = int.Parse(userIdClaim.Value);

            //var userId = int.Parse(User.FindFirst("id").Value);

            var cartItems = await context.CartItems
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

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            // Process each cart item
            foreach (var item in cartItems)
            {
                // Update stock
                item.Product.Stock -= item.Quantity;

                // Create order item
                context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = item.Product.Price
                });
            }

            // Clear cart
            context.CartItems.RemoveRange(cartItems);
            await context.SaveChangesAsync();

            return Ok(new
            {
                orderId = order.Id,
                message = "Order created successfully"
            });
        }

        [HttpGet("my")]
        public async Task<IActionResult> MyOrders()
        {
            var userId = int.Parse(User.FindFirst("id").Value);

            var orders = await context.Orders
                .Where(o => o.UserId == userId)
                .Select(o => new
                {
                    o.Id,
                    o.TotalPrice,
                    o.Status,
                    o.CreatedAt,
                    o.ShippingAddress,
                    o.Phone,
                    ItemCount = context.OrderItems.Count(i => i.OrderId == o.Id)
                })
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> OrderDetails(int id)
        {
            var userId = int.Parse(User.FindFirst("id").Value);

            var order = await context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null)
                return NotFound("Order not found");

            var items = await context.OrderItems
                .Where(i => i.OrderId == id)
                .Select(i => new
                {
                    i.Id,
                    i.ProductId,
                    i.Quantity,
                    i.PriceAtPurchase,
                    Subtotal = i.Quantity * i.PriceAtPurchase,
                    ProductName = context.Products
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