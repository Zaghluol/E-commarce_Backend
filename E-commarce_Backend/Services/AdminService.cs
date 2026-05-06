using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos.Admin;
using E_commarce_Backend.Dtos.support;
using E_commarce_Backend.Models.Enums;
using E_commarce_Backend.Models.order;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Services
{
    public class AdminService : IAdminService
    {
        private readonly ECommerceDbContext context;

        public AdminService(ECommerceDbContext context)
        {
            this.context = context;
        }

        // =========================
        // 📊 DASHBOARD
        // =========================
        public async Task<AdminDashboardDto> GetDashboardAsync()
        {
            return new AdminDashboardDto
            {
                TotalUsers = await context.Users.CountAsync(),

                TotalOrders = await context.Orders.CountAsync(),

                TotalRevenue = await context.Orders
                    .Where(o => o.Status == OrderStatus.Paid)
                    .SumAsync(o => (decimal?)o.TotalAmount) ?? 0,

                PendingPayments = await context.Orders
                    .CountAsync(o => o.Status == OrderStatus.PendingPayment),

                ProcessingOrders = await context.Orders
                    .CountAsync(o => o.Status == OrderStatus.Processing),

                ShippedOrders = await context.Orders
                    .CountAsync(o => o.Status == OrderStatus.Shipped),

                DeliveredOrders = await context.Orders
                    .CountAsync(o => o.Status == OrderStatus.Delivered),

                FailedOrders = await context.Orders
                    .CountAsync(o => o.Status == OrderStatus.Failed)
            };
        }

        // =========================
        // 👥 USERS
        // =========================
        public async Task<List<AdminUserDto>> GetUsersAsync()
        {
            return await context.Users
                .Select(u => new AdminUserDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber
                })
                .ToListAsync();
        }

        // =========================
        // 📦 ORDERS (ADMIN VIEW)
        // =========================
        public async Task<List<AdminOrderDto>> GetOrdersAsync()
        {
            return await context.Orders
                .Where(o => o.Status != OrderStatus.PendingPayment) // 🔥 hide unpaid
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new AdminOrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();
        }

        // =========================
        // 🔁 UPDATE STATUS (ENUM)
        // =========================
        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await context.Orders.FindAsync(orderId);

            if (order == null)
                throw new Exception("Order not found");

            // 🔥 STRICT ORDER FLOW
            switch (status)
            {
                case OrderStatus.Processing:
                    if (order.Status != OrderStatus.Paid)
                        throw new Exception("Only Paid orders can move to Processing");
                    break;

                case OrderStatus.Shipped:
                    if (order.Status != OrderStatus.Processing)
                        throw new Exception("Order must be Processing first");
                    break;

                case OrderStatus.Delivered:
                    if (order.Status != OrderStatus.Shipped)
                        throw new Exception("Order must be Shipped first");
                    break;
            }

            order.Status = status;

            context.OrderStatusHistories.Add(new OrderStatusHistory
            {
                OrderId = orderId,
                Status = status,
                CreatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        // =========================
        // 🔁 UPDATE STATUS (STRING → ENUM)
        // =========================
        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            if (!Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
                throw new ArgumentException("Invalid order status", nameof(status));

            await UpdateOrderStatusAsync(orderId, parsedStatus);
        }

        // =========================
        // 💬 SUPPORT
        // =========================
        public async Task<List<ConversationDto>> GetSupportConversationsAsync()
        {
            return await context.Conversations
                .Include(c => c.Messages)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new ConversationDto
                {
                    Id = c.Id,
                    IsClosed = c.IsClosed,
                    Messages = c.Messages.Select(m => new MessageDto
                    {
                        Content = m.Content,
                        IsFromAdmin = m.IsFromAdmin,
                        CreatedAt = m.CreatedAt
                    }).ToList()
                })
                .ToListAsync();
        }
    }
}