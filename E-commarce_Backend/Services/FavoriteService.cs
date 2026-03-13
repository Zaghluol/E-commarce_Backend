using E_commarce_Backend.Dtos.Favorite;
using E_commarce_Backend.Models;
using E_commarce_Backend.Repository;
using E_commarce_Backend.Services.Abstractions;

namespace E_commarce_Backend.Services
{
    public class FavoriteService(IFavoriteRepository favoriteRepository) : IFavoriteService
    {
        public async Task<bool> ToggleFavoriteAsync(string userId, int productId)
        {
            var favorite = await favoriteRepository.GetAsync(userId, productId);

            if (favorite != null)
            {
                await favoriteRepository.RemoveAsync(favorite);
                await favoriteRepository.SaveAsync();

                return false; // removed from favorites
            }

            var newFavorite = new Favorite
            {
                UserId = userId,
                ProductId = productId
            };

            await favoriteRepository.AddAsync(newFavorite);
            await favoriteRepository.SaveAsync();

            return true; // added to favorites
        }
        public async Task<List<FavoriteDto>> GetUserFavoritesAsync(string userId)
        {
            var favorites = await favoriteRepository.GetUserFavoritesAsync(userId);

            return favorites.Select(f => new FavoriteDto
            {
                Id = f.Id,
                ProductId = f.ProductId,
                ProductName = f.Product.Name,
                Price = f.Product.Price,
                ImageUrl = f.Product.ImageUrl
            }).ToList();
        }

        public async Task ClearFavoritesAsync(string userId)
        {
            var favorites = await favoriteRepository.GetUserFavoritesAsync(userId);

            await favoriteRepository.RemoveRangeAsync(favorites);

            await favoriteRepository.SaveAsync();
        }
        public async Task<bool> IsFavoriteAsync(string userId, int productId)
        {
            var favorite = await favoriteRepository.GetAsync(userId, productId);

            return favorite != null;
        }
    }
}
