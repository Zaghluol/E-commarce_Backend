using E_commarce_Backend.Dtos.Favorite;

namespace E_commarce_Backend.Services.Abstractions
{
    public interface IFavoriteService
    {
        Task<bool> ToggleFavoriteAsync(string userId, int productId);

        Task<List<FavoriteDto>> GetUserFavoritesAsync(string userId);

        Task ClearFavoritesAsync(string userId);
        Task<bool> IsFavoriteAsync(string userId, int productId);
    }
}
