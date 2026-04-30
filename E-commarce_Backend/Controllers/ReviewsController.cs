using E_commarce_Backend.Dtos.Reviews;
using E_commarce_Backend.Services.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_commarce_Backend.Controllers
{
    [ApiController]
    [Route("api/reviews")]
    [Authorize]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService service;

        public ReviewsController(IReviewService service)
        {
            this.service = service;
        }

        private string GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
        }

        // 📥 Get reviews for product
        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductReviews(int productId)
        {
            var result = await service.GetProductReviewsAsync(productId);
            return Ok(result);
        }

        // ➕ Add review
        [HttpPost]
        public async Task<IActionResult> AddReview(CreateReviewDto dto)
        {
            await service.AddReviewAsync(GetUserId(), dto);
            return Ok();
        }

        // ❓ Can user review?
        [HttpGet("can-review")]
        public async Task<IActionResult> CanReview(int productId)
        {
            var result = await service.CanUserReviewAsync(GetUserId(), productId);
            return Ok(result);
        }
    }
}
