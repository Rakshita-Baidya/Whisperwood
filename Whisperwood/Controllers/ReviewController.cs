using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : BaseController
    {
        private readonly IReviewService reviewService;

        public ReviewController(IReviewService reviewService)
        {
            this.reviewService = reviewService;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddReview(ReviewDTO dto)
        {
            var userId = GetLoggedInUserId();
            return await reviewService.AddReviewAsync(userId, dto);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllReviews()
        {
            return await reviewService.GetAllReviewsAsync();
        }

        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetReviewsByBook(Guid bookId)
        {
            return await reviewService.GetReviewsByBookAsync(bookId);
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(Guid id, ReviewUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            return await reviewService.UpdateReviewAsync(userId, id, dto);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await reviewService.DeleteReviewAsync(userId, id);
        }

        [HttpGet("getbyuserid")]
        public async Task<IActionResult> GetByUserId()
        {
            var userId = GetLoggedInUserId();
            var review = await reviewService.GetByUserIdAsync(userId);
            if (review == null)
                return NotFound("Reviews not found.");
            return Ok(review);
        }
    }
}
