using E_commarce_Backend.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_commarce_Backend.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController(INotificationService service) : ControllerBase
    {
        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }

        // 📥 Get all notifications
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var result = await service.GetUserNotificationsAsync(GetUserId());
            return Ok(result);
        }

        // ✅ Mark one as read
        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await service.MarkAsReadAsync(id, GetUserId());
            return Ok();
        }

        // ✅ Mark all as read
        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await service.MarkAllAsReadAsync(GetUserId());
            return Ok();
        }
    }
}
