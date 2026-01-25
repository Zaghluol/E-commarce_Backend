using E_commarce_Backend.Dtos;
using E_commarce_Backend.Models;

namespace E_commarce_Backend.Services.Abstractions
{
    public interface ICartService
    {
        Task<CartDto> GetCartAsync(string userId);
        Task<CartDto> AddToCartAsync(string userId, int productId, int quantity);
        Task UpdateItemAsync(string userId, int itemId, int quantity);
        Task RemoveItemAsync(string userId, int itemId);
        Task ClearCartAsync(string userId);


    }

}
