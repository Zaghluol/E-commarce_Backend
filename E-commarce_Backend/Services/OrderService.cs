using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos.Orders;
using E_commarce_Backend.Models;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Services
{
    public class OrderService(ECommerceDbContext context, IPaymobService paymobService) : IOrderService
    {
        // 🛒 Checkout
        public async Task<object> CheckoutAsync(string userId, CheckoutDto dto)
        {
            var cartItems = await context.CartItems
                .Include(c => c.Product)
                .Include(c => c.Cart)
                .Where(c => c.Cart.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                throw new Exception("Cart is empty");

            decimal total = 0;
            var stockIssues = new List<string>();

            foreach (var item in cartItems)
            {
                if (item.Product.Stock < item.Quantity)
                    stockIssues.Add($"Not enough stock for {item.Product.Name}");

                total += item.Product.Price * item.Quantity;
            }

            if (stockIssues.Any())
                throw new Exception(string.Join(", ", stockIssues));

            // 🧾 Create Order
            var order = new Order
            {
                UserId = userId,
                TotalPrice = total,
                ShippingAddress = dto.ShippingAddress,
                Phone = dto.Phone,
                Status = dto.PaymentMethod == "card" ? "PendingPayment" : "Pending",
                CreatedAt = DateTime.UtcNow
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            // 📦 Create OrderItems + update stock
            foreach (var item in cartItems)
            {
                item.Product.Stock -= item.Quantity;

                context.OrderItems.Add(new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    PriceAtPurchase = item.Product.Price
                });
            }

            // 🧹 Clear cart
            context.CartItems.RemoveRange(cartItems);
            await context.SaveChangesAsync();

            // 💳 Payment (Paymob)
            if (dto.PaymentMethod == "card")
            {
                var paymentUrl = await paymobService.CreatePaymentUrl(
                    total,
                    order.Id.ToString(),
                    "test@email.com" // replace later with real user email
                );

                return new
                {
                    message = "Redirect to payment",
                    orderId = order.Id,
                    paymentUrl
                };
            }

            // 💵 Cash
            return new
            {
                message = "Order placed successfully",
                orderId = order.Id,
                total
            };
        }

        // 🧾 Get My Orders
        public async Task<object> GetMyOrdersAsync(string userId)
        {
            return await context.Orders
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
        }

        // 📦 Order Details
        public async Task<object> GetOrderDetailsAsync(string userId, int orderId)
        {
            var order = await context.Orders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                throw new Exception("Order not found");

            var items = order.Items.Select(i => new
            {
                i.Id,
                i.ProductId,
                i.Quantity,
                i.PriceAtPurchase,
                Subtotal = i.Quantity * i.PriceAtPurchase,
                ProductName = i.Product.Name
            });

            return new
            {
                Order = order,
                Items = items,
                Total = order.TotalPrice
            };
        }
    }
}