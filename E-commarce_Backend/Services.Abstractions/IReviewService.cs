using E_commarce_Backend.Dtos.Reviews;

namespace E_commarce_Backend.Services.Abstractions
{
    public interface IReviewService
    {
        Task<List<ReviewDto>> GetProductReviewsAsync(int productId);

        Task AddReviewAsync(string userId, CreateReviewDto dto);

        Task<bool> CanUserReviewAsync(string userId, int productId);
    }
}
