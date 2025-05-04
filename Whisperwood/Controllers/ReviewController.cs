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
            return await reviewService.AddReview(userId, dto);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllReviews()
        {
            return await reviewService.GetAllReviews();
        }

        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetReviewsByBook(Guid bookId)
        {
            return await reviewService.GetReviewsByBook(bookId);
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(Guid id, ReviewUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            return await reviewService.UpdateReview(userId, id, dto);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await reviewService.DeleteReview(userId, id);
        }
    }
}
