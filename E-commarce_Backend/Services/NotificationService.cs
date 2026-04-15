using E_commarce_Backend.Data;
using E_commarce_Backend.Models;
using E_commarce_Backend.Services.Abstractions;

namespace E_commarce_Backend.Services
{
    public class NotificationService(ECommerceDbContext context) : INotificationService
    {
        public async Task SendAsync(string userId, string title, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message
            };

            context.Notifications.Add(notification);
            await context.SaveChangesAsync();

            // 🔥 TEMP: simulate push
            Console.WriteLine($"🔔 Notification to {userId}: {title} - {message}");
        }
    }
}
