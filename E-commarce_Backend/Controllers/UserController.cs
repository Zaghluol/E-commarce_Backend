namespace E_commarce_Backend.Controllers
{
    using System.Security.Claims;
    using E_commarce_Backend.Dtos.Profile;
    using E_commarce_Backend.Services.Abstractions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController(IUserService service) : ControllerBase
    {

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }

        // 👤 GET profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var result = await service.GetProfileAsync(GetUserId());
            return Ok(result);
        }

        // ✏️ UPDATE profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            await service.UpdateProfileAsync(GetUserId(), dto);
            return Ok();
        }
    }
}
