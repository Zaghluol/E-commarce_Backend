namespace E_commarce_Backend.Controllers
{
    using E_commarce_Backend.Services.Abstractions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController(IAdminService service) : ControllerBase
    {
        // 📊 Dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboard()
        {
            var result = await service.GetDashboardAsync();
            return Ok(result);
        }

        // 👥 Users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await service.GetUsersAsync();
            return Ok(result);
        }

        // 📦 Orders
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders()
        {
            var result = await service.GetOrdersAsync();
            return Ok(result);
        }

        // 🔄 Update order status
        [HttpPut("orders/{id}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] string status)
        {
            await service.UpdateOrderStatusAsync(id, status);
            return Ok();
        }

        // 💬 Support
        [HttpGet("support")]
        public async Task<IActionResult> GetSupport()
        {
            var result = await service.GetSupportConversationsAsync();
            return Ok(result);
        }
    }
}
