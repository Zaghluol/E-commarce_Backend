using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos.Orders;
using E_commarce_Backend.Models;
using E_commarce_Backend.Models.order;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Services
{
    public class OrderService(ECommerceDbContext context,
        IPaymobService paymobService
        ,ICouponService couponService,
        INotificationService notificationService,
        IConfiguration config) : IOrderService
    {
        private async Task AddStatusAsync(int orderId, string status)
        {
            var history = new OrderStatusHistory
            {
                OrderId = orderId,
                Status = status,
                Date = DateTime.UtcNow
            };

            context.OrderStatusHistories.Add(history);
            await context.SaveChangesAsync();
        }
        // 🛒 Checkout
        public async Task<string> CheckoutAsync(string userId, CheckoutDto dto)
        {
            // 1️⃣ Get user cart
            var cartItems = await context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                throw new Exception("Cart is empty");

            // 2️⃣ Calculate total
            var total = cartItems.Sum(c => c.Quantity * c.Product.Price);

            // 3️⃣ Create Order
            var order = new Order
            {
                UserId = userId,
                TotalAmount = total,
                Status = "PendingPayment",
                CreatedAt = DateTime.UtcNow,
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Price = c.Product.Price
                }).ToList()
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            // 4️⃣ Determine Payment Method
            var paymentMethod = await context.PaymentMethods
                .FirstOrDefaultAsync(x => x.UserId == userId && x.IsDefault);

            // fallback if none selected
            var methodType = paymentMethod?.Type ?? "Card";

            // 5️⃣ Choose Paymob Integration
            int integrationId = methodType switch
            {
                "Wallet" => int.Parse(config["Paymob:WalletIntegrationId"]),
                "Fawry" => int.Parse(config["Paymob:FawryIntegrationId"]),
                _ => int.Parse(config["Paymob:CardIntegrationId"])
            };

            // 6️⃣ Call Paymob
            (string paymentUrl, string paymobOrderId) =
            await paymobService.CreatePaymentUrl(
                total,
                order.Id.ToString(),
                dto.Email,
                integrationId);

            // 🔴 IMPORTANT: store Paymob reference
            order.PaymentRef = paymobOrderId;

            await context.SaveChangesAsync();

            // 7️⃣ Clear cart
            context.CartItems.RemoveRange(cartItems);
            await context.SaveChangesAsync();

            return paymentUrl;
        }

        // 🧾 Get My Orders
        public async Task<object> GetMyOrdersAsync(string userId)
        {
            return await context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new
                {
                    o.Id,
                    o.TotalAmount,
                    o.Status,
                    o.CreatedAt,
                    o.ShippingAddress,
                    o.Phone,
                    ItemCount = o.OrderItems.Count
                })
                .ToListAsync();
        }

        // 📦 Order Details
        public async Task<object> GetOrderDetailsAsync(string userId, int orderId)
        {
            var order = await context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                throw new Exception("Order not found");

            var items = order.OrderItems.Select(i => new
            {
                i.Id,
                i.ProductId,
                i.Quantity,
                i.Price,
                Subtotal = i.Quantity * i.Price,
                ProductName = i.Product.Name
            });

            return new
            {
                Order = order,
                Items = items,
                Total = order.TotalAmount
            };
        }
        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await context.Orders.FindAsync(orderId);

            if (order == null)
                throw new Exception("Order not found");

            order.Status = status;

            await context.SaveChangesAsync();

            await AddStatusAsync(orderId, status);

            // 🔔 SEND NOTIFICATION
            await notificationService.SendAsync(
                order.UserId,
                "Order Update",
                $"Your order #{order.Id} is now {status}"
            );
        }
    }
}