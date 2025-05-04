using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;
using Whisperwood.Models;

namespace Whisperwood.Services
{
    public class ReviewService : IReviewService
    {
        private readonly WhisperwoodDbContext dbContext;

        public ReviewService(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> AddReview(Guid userId, ReviewDTO dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
                return new BadRequestObjectResult("User not found.");

            var book = await dbContext.Books.FindAsync(dto.BookId);
            if (book == null)
                return new NotFoundObjectResult("Book not found.");

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
            return new OkObjectResult(review);
        }

        public async Task<IActionResult> GetAllReviews()
        {
            var reviews = await dbContext.Reviews
                .Include(r => r.Users)
                .Include(r => r.Books)
                .ToListAsync();

            return new OkObjectResult(reviews);
        }

        public async Task<IActionResult> GetReviewsByBook(Guid bookId)
        {
            var book = await dbContext.Books.FindAsync(bookId);
            if (book == null)
                return new NotFoundObjectResult("Book not found.");

            var reviews = await dbContext.Reviews
                .Where(r => r.BookId == bookId)
                .Include(r => r.Users)
                .ToListAsync();

            return new OkObjectResult(reviews);
        }

        public async Task<IActionResult> UpdateReview(Guid userId, Guid id, ReviewUpdateDto dto)
        {
            var review = await dbContext.Reviews.FindAsync(id);
            if (review == null)
                return new NotFoundObjectResult("Review not found.");

            if (review.UserId != userId)
                return new UnauthorizedObjectResult("You can only update your own reviews.");

            if (dto.BookId != null)
            {
                var bookExists = await dbContext.Books.AnyAsync(b => b.Id == dto.BookId);
                if (!bookExists) return new NotFoundObjectResult("Book not found.");
                review.BookId = dto.BookId.Value;
            }

            if (dto.Rating != null) review.Rating = dto.Rating.Value;
            if (dto.Message != null) review.Message = dto.Message;

            review.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();
            return new OkObjectResult(review);
        }

        public async Task<IActionResult> DeleteReview(Guid userId, Guid id)
        {
            var review = await dbContext.Reviews.FindAsync(id);
            if (review == null)
                return new NotFoundObjectResult("Review not found.");

            if (review.UserId != userId)
                return new UnauthorizedObjectResult("You can only delete your own reviews.");

            dbContext.Reviews.Remove(review);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult("Review deleted successfully.");
        }
    }
}
