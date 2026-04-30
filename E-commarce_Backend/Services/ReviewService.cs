using E_commarce_Backend.Data;
using E_commarce_Backend.Dtos.Reviews;
using E_commarce_Backend.Models;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace E_commarce_Backend.Services
{
    public class ReviewService : IReviewService
    {
        private readonly ECommerceDbContext context;

        public ReviewService(ECommerceDbContext context)
        {
            this.context = context;
        }

        public async Task<List<ReviewDto>> GetProductReviewsAsync(int productId)
        {
            return await context.Reviews
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    UserName = r.UserId, // you can replace with Identity username later
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        public async Task AddReviewAsync(string userId, CreateReviewDto dto)
        {
            // 🔴 prevent duplicate review
            var alreadyReviewed = await context.Reviews
                .AnyAsync(r => r.UserId == userId && r.ProductId == dto.ProductId);

            if (alreadyReviewed)
                throw new Exception("You already reviewed this product");

            var review = new Review
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Rating = dto.Rating,
                Comment = dto.Comment
            };

            context.Reviews.Add(review);
            await context.SaveChangesAsync();
        }

        public async Task<bool> CanUserReviewAsync(string userId, int productId)
        {
            // MUST have purchased product first
            var hasPurchased = await context.Orders
                .AnyAsync(o =>
                    o.UserId == userId &&
                    o.Status == "Paid" &&
                    o.OrderItems.Any(i => i.ProductId == productId));

            if (!hasPurchased)
                return false;

            // not already reviewed
            var alreadyReviewed = await context.Reviews
                .AnyAsync(r => r.UserId == userId && r.ProductId == productId);

            return !alreadyReviewed;
        }
    }
}
