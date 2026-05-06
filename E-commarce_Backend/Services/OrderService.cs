using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos.Orders;
using E_commarce_Backend.Models.order;
using E_commarce_Backend.Models.Enums;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Services
{
    public class OrderService : IOrderService
    {
        private readonly ECommerceDbContext context;
        private readonly IPaymobService paymobService;
        private readonly INotificationService notificationService;

        public OrderService(
            ECommerceDbContext context,
            IPaymobService paymobService,
            INotificationService notificationService)
        {
            this.context = context;
            this.paymobService = paymobService;
            this.notificationService = notificationService;
        }

        // =========================
        // 📌 Add Status History
        // =========================
        private async Task AddStatusAsync(int orderId, OrderStatus status)
        {
            context.OrderStatusHistories.Add(new OrderStatusHistory
            {
                OrderId = orderId,
                Status = status,
                CreatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        // =========================
        // 🛒 Checkout
        // =========================
        public async Task<object> CheckoutAsync(string userId, CheckoutDto dto)
        {
            var cartItems = await context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                throw new Exception("Cart is empty");

            var total = cartItems.Sum(x => x.Quantity * x.Product.Price);

            var order = new Order
            {
                UserId = userId,
                TotalAmount = total,
                Status = OrderStatus.PendingPayment,
                ShippingAddress = dto.ShippingAddress,
                Phone = dto.Phone,
                OrderItems = cartItems.Select(c => new OrderItem
                {
                    ProductId = c.ProductId,
                    Quantity = c.Quantity,
                    Price = c.Product.Price
                }).ToList()
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            // 🔥 Paymob
            var paymentUrl = await paymobService.CreatePaymentUrl(
                order.TotalAmount,
                order.Id.ToString(),
                dto.Email
            );

            await AddStatusAsync(order.Id, OrderStatus.PendingPayment);

            return new
            {
                OrderId = order.Id,
                PaymentUrl = paymentUrl
            };
        }

        // =========================
        // 📦 Get My Orders
        // =========================
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
                    Status = o.Status.ToString(),
                    o.CreatedAt,
                    o.ShippingAddress,
                    o.Phone,
                    ItemCount = o.OrderItems.Count
                })
                .ToListAsync();
        }

        // =========================
        // 📦 Order Details
        // =========================
        public async Task<object> GetOrderDetailsAsync(string userId, int orderId)
        {
            var order = await context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

            if (order == null)
                throw new Exception("Order not found");

            return new
            {
                OrderId = order.Id,
                Status = order.Status.ToString(),
                order.TotalAmount,
                order.CreatedAt,
                order.ShippingAddress,
                order.Phone,
                Items = order.OrderItems.Select(i => new
                {
                    i.ProductId,
                    i.Quantity,
                    i.Price,
                    ProductName = i.Product.Name,
                    Subtotal = i.Quantity * i.Price
                })
            };
        }

        // =========================
        // 🔁 Update Status (Webhook / Internal)
        // =========================
        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await context.Orders.FindAsync(orderId);

            if (order == null)
                throw new Exception("Order not found");

            // 🔥 RULES
            if (status == OrderStatus.Processing && order.Status != OrderStatus.Paid)
                throw new Exception("Order must be Paid first");

            order.Status = status;

            await context.SaveChangesAsync();
            await AddStatusAsync(orderId, status);

            await notificationService.SendAsync(
                order.UserId,
                "Order Update",
                $"Your order #{order.Id} is now {status}"
            );
        }
    }
}