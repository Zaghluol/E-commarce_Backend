using E_commarce_Backend.Dtos.NotificationSettings;

namespace E_commarce_Backend.Services.Abstractions
{
    public interface INotificationSettingsService
    {
        Task<NotificationSettingsDto> GetSettingsAsync(string userId);
        Task UpdateSettingsAsync(string userId, NotificationSettingsDto dto);
    }
}
