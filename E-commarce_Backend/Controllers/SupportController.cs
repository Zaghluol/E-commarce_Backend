using E_commarce_Backend.Dtos.support;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_commarce_Backend.Controllers
{
    [ApiController]
    [Route("api/support")]
    [Authorize]
    public class SupportController(ISupportService service) : ControllerBase
    {
        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }

        // 📡 Channels
        [HttpGet("channels")]
        [AllowAnonymous]
        public async Task<IActionResult> GetChannels()
        {
            return Ok(await service.GetChannelsAsync());
        }

        // 💬 Conversations
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            return Ok(await service.GetUserConversationsAsync(GetUserId()));
        }

        // ✉️ Send message
        [HttpPost("messages")]
        public async Task<IActionResult> SendMessage(SendMessageDto dto)
        {
            await service.SendMessageAsync(GetUserId(), dto);
            return Ok();
        }
    }
}
