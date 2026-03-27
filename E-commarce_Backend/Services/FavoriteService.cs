using E_commarce_Backend.Dtos.Favorite;
using E_commarce_Backend.Models;
using E_commarce_Backend.Repository;
using E_commarce_Backend.Services.Abstractions;

namespace E_commarce_Backend.Services
{
    public class FavoriteService(IFavoriteRepository favoriteRepository) : IFavoriteService
    {
        // 🔄 Toggle favorite
        public async Task<bool> ToggleFavoriteAsync(string userId, int productId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");

            var favorite = await favoriteRepository.GetAsync(userId, productId);

            if (favorite != null)
            {
                await favoriteRepository.RemoveAsync(favorite);
                await favoriteRepository.SaveAsync();
                return false; // removed
            }

            var newFavorite = new Favorite
            {
                UserId = userId,
                ProductId = productId
            };

            await favoriteRepository.AddAsync(newFavorite);
            await favoriteRepository.SaveAsync();

            return true; // added
        }

        // 📦 Get user favorites
        public async Task<List<FavoriteDto>> GetUserFavoritesAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");

            var favorites = await favoriteRepository.GetUserFavoritesAsync(userId);

            return favorites.Select(f => new FavoriteDto
            {
                Id = f.Id,
                ProductId = f.ProductId,
                ProductName = f.Product?.Name ?? "",
                Price = f.Product?.Price ?? 0,
                ImageUrl = f.Product?.ImageUrl ?? ""
            }).ToList();
        }

        // 🧹 Clear favorites
        public async Task ClearFavoritesAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");

            var favorites = await favoriteRepository.GetUserFavoritesAsync(userId);

            if (!favorites.Any())
                return;

            await favoriteRepository.RemoveRangeAsync(favorites);
            await favoriteRepository.SaveAsync();
        }

        // ⭐ Check if product is favorite
        public async Task<bool> IsFavoriteAsync(string userId, int productId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");

            var favorite = await favoriteRepository.GetAsync(userId, productId);

            return favorite != null;
        }
    }
}