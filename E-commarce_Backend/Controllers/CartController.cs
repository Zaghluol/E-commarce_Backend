using E_commarce_Backend.Dtos;
using E_commarce_Backend.Models;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_commarce_Backend.Controllers
{
    [ApiController]
    [Route("api/cart")]
    //[Authorize]
    public class CartController(ICartService cartService, UserManager<AppUser> userManager) : ControllerBase
    {

        private string UserId => userManager.GetUserId(User)!;

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
           var Carts = await cartService.GetCartAsync(UserId);
            return Ok(Carts);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(AddToCartDto dto)
        {
          var Adding = await cartService.AddToCartAsync(UserId, dto.ProductId, dto.Quantity);
            return Ok(Adding);
        }

        [HttpPut("{itemId}")]
        public async Task<IActionResult> UpdateItem(int itemId, UpdateCartItemDto dto)
        {
            await cartService.UpdateItemAsync(UserId, itemId, dto.Quantity);
            return Ok();
        }

        [HttpDelete("{itemId}")]
        public async Task<IActionResult> RemoveItem(int itemId)
        {
            await cartService.RemoveItemAsync(UserId, itemId);
            return Ok();
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> ClearCart()
        {
            await cartService.ClearCartAsync(UserId);
            return Ok();
        }
    }


}
