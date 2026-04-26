using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos.Notifications;
using E_commarce_Backend.Models;
using E_commarce_Backend.Models.Nofications;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Services
{
    public class NotificationSettingsService(ECommerceDbContext context) : INotificationSettingsService
    {
        public async Task<NotificationSettingsDto> GetSettingsAsync(string userId)
        {
            var settings = await context.NotificationSettings
                .FirstOrDefaultAsync(ns => ns.UserId == userId);

            if (settings == null)
            {
                // Create default settings if none exist
                settings = new NotificationSettings { UserId = userId };
                context.NotificationSettings.Add(settings);
                await context.SaveChangesAsync();
            }

            return new NotificationSettingsDto
            {
                EmailNotificationsEnabled = settings.EmailNotificationsEnabled,
                PushNotificationsEnabled = settings.PushNotificationsEnabled,
                OrderStatusUpdatesEnabled = settings.OrderStatusUpdatesEnabled,
                PromotionsEnabled = settings.PromotionsEnabled,
                ProductRestockAlertsEnabled = settings.ProductRestockAlertsEnabled
            };
        }

        public async Task UpdateSettingsAsync(string userId, NotificationSettingsDto dto)
        {
            var settings = await context.NotificationSettings
                .FirstOrDefaultAsync(ns => ns.UserId == userId);

            if (settings == null)
            {
                settings = new NotificationSettings { UserId = userId };
                context.NotificationSettings.Add(settings);
            }

            settings.EmailNotificationsEnabled = dto.EmailNotificationsEnabled;
            settings.PushNotificationsEnabled = dto.PushNotificationsEnabled;
            settings.OrderStatusUpdatesEnabled = dto.OrderStatusUpdatesEnabled;
            settings.PromotionsEnabled = dto.PromotionsEnabled;
            settings.ProductRestockAlertsEnabled = dto.ProductRestockAlertsEnabled;

            await context.SaveChangesAsync();
        }
    }
}
