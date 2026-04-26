using E_commarce_Backend.Dtos.Notifications;

namespace E_commarce_Backend.Services.Abstractions
{
    public interface INotificationService
    {
        Task SendAsync(string userId, string title, string message);

        Task<List<NotificationDto>> GetUserNotificationsAsync(string userId);

        Task MarkAsReadAsync(int notificationId, string userId);

        Task MarkAllAsReadAsync(string userId);
    }
}
