using E_commarce_Backend.Data;
using E_commarce_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Repository
{
    public class FavoriteRepository(ECommerceDbContext context) : IFavoriteRepository
    {

        public async Task<Favorite?> GetAsync(string userId, int productId)
        {
            return await context.Favorites
                .FirstOrDefaultAsync(f => f.UserId == userId && f.ProductId == productId);
        }

        public async Task AddAsync(Favorite favorite)
        {
            await context.Favorites.AddAsync(favorite);
        }

        public async Task RemoveAsync(Favorite favorite)
        {
            context.Favorites.Remove(favorite);
            await Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }
        public async Task<List<Favorite>> GetUserFavoritesAsync(string userId)
        {
            return await context.Favorites
                .Include(f => f.Product)
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task RemoveRangeAsync(List<Favorite> favorites)
        {
            context.Favorites.RemoveRange(favorites);
            await Task.CompletedTask;
        }
    }
}
