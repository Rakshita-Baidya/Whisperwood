using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Models;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : BaseController
    {
        private readonly WhisperwoodDbContext dbContext;

        public ReviewController(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddReview(ReviewDTO dto)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
                return BadRequest("User not found.");

            var book = await dbContext.Books.FindAsync(dto.BookId);
            if (book == null)
                return NotFound("Book not found.");

            var review = new Reviews
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BookId = dto.BookId,
                Rating = dto.Rating,
                Message = dto.Message ?? "",
                CreatedAt = DateTime.UtcNow
            };

            dbContext.Reviews.Add(review);
            await dbContext.SaveChangesAsync();
            return Ok(review);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await dbContext.Reviews
                .Include(r => r.Users)
                .Include(r => r.Books)
                .ToListAsync();

            return Ok(reviews);
        }

        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetReviewsByBook(Guid bookId)
        {
            var book = await dbContext.Books.FindAsync(bookId);
            if (book == null)
                return NotFound("Book not found.");

            var reviews = await dbContext.Reviews
                .Where(r => r.BookId == bookId)
                .Include(r => r.Users)
                .ToListAsync();

            return Ok(reviews);
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(Guid id, ReviewUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            var review = await dbContext.Reviews.FindAsync(id);
            if (review == null)
                return NotFound("Review not found.");

            if (review.UserId != userId)
                return Unauthorized("You can only update your own reviews.");

            if (dto.BookId != null)
            {
                var bookExists = await dbContext.Books.AnyAsync(b => b.Id == dto.BookId);
                if (!bookExists) return NotFound("Book not found.");
                review.BookId = dto.BookId.Value;
            }

            if (dto.Rating != null) review.Rating = dto.Rating.Value;
            if (dto.Message != null) review.Message = dto.Message;

            review.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();
            return Ok(review);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            var userId = GetLoggedInUserId();
            var review = await dbContext.Reviews.FindAsync(id);

            if (review == null)
                return NotFound("Review not found.");

            if (review.UserId != userId)
                return Unauthorized("You can only delete your own reviews.");

            dbContext.Reviews.Remove(review);
            await dbContext.SaveChangesAsync();
            return Ok("Review deleted successfully.");
        }
    }
}
