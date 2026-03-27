using System.Security.Claims;
using E_commarce_Backend.Models;
using E_commarce_Backend.Services;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace E_commarce_Backend.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritesController(IFavoriteService favoriteService,UserManager<AppUser> userManager) : ControllerBase
    {

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpPost("toggle/{productId}")]
        public async Task<IActionResult> ToggleFavorite(int productId)
        {
            var userId = GetUserId();

            var result = await favoriteService.ToggleFavoriteAsync(userId, productId);

            return Ok(new
            {
                isFavorite = result
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = GetUserId();
            var favorites = await favoriteService.GetUserFavoritesAsync(userId);

            return Ok(favorites);
        }
        [HttpDelete("clear")]
        public async Task<IActionResult> ClearFavorites()
        {
            var userId = GetUserId();

            await favoriteService.ClearFavoritesAsync(userId);

            return Ok("Favorites cleared");
        }
        [HttpGet("check/{productId}")]
        public async Task<IActionResult> CheckFavorite(int productId)
        {
            var userId = GetUserId();

            var result = await favoriteService.IsFavoriteAsync(userId, productId);

            return Ok(result);
        }
    }
}
