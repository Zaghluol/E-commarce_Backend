using E_commarce_Backend.Dtos.Notifications;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_commarce_Backend.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class NotificationSettingsController(INotificationSettingsService settingsService) : ControllerBase
    {
        private string GetUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User ID not found");
            return userId;
        }

        // GET /api/User/notification-settings
        [HttpGet("notification-settings")]
        public async Task<IActionResult> GetNotificationSettings()
        {
            var settings = await settingsService.GetSettingsAsync(GetUserId());
            return Ok(settings);
        }

        // PUT /api/User/notification-settings
        [HttpPut("notification-settings")]
        public async Task<IActionResult> UpdateNotificationSettings(NotificationSettingsDto dto)
        {
            await settingsService.UpdateSettingsAsync(GetUserId(), dto);
            return NoContent();
        }
    }
}
