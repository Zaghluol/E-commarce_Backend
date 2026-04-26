using E_commarce_Backend.Dtos.Notifications;

namespace E_commarce_Backend.Services.Abstractions
{
    public interface INotificationSettingsService
    {
        Task<NotificationSettingsDto> GetSettingsAsync(string userId);
        Task UpdateSettingsAsync(string userId, NotificationSettingsDto dto);
    }
}
