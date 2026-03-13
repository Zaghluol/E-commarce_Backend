using E_commarce_Backend.Models;

namespace E_commarce_Backend.Repository
{
    public interface IFavoriteRepository
    {
        Task<Favorite?> GetAsync(string userId, int productId);

        Task<List<Favorite>> GetUserFavoritesAsync(string userId);

        Task AddAsync(Favorite favorite);

        Task RemoveAsync(Favorite favorite);

        Task RemoveRangeAsync(List<Favorite> favorites);

        Task SaveAsync();
    }
}
