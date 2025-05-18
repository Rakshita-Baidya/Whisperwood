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

        public async Task<IActionResult> AddReviewAsync(Guid userId, ReviewDTO dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
                return new BadRequestObjectResult(new
                {
                    message = "User not found."
                });

            var book = await dbContext.Books.FindAsync(dto.BookId);
            if (book == null)
                return new NotFoundObjectResult(new
                {
                    message = "Book not found."
                });

            var hasFulfilledOrder = await dbContext.OrderItem
                .Where(oi => oi.BookId == dto.BookId && oi.Order.UserId == userId && oi.Order.Status == Orders.OrderStatus.Fulfilled)
                .AnyAsync();
            if (!hasFulfilledOrder)
                return new UnauthorizedObjectResult(new
                {
                    message = "You can only review books you have purchased and received."
                });

            var existingReview = await dbContext.Reviews
                .AnyAsync(r => r.UserId == userId && r.BookId == dto.BookId);
            if (existingReview)
                return new ConflictObjectResult(new
                {
                    message = "You have already submitted a review for this book."
                });


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
            await UpdateAverageRatingAsync(dto.BookId);
            return new OkObjectResult(review);
        }

        public async Task<IActionResult> GetAllReviewsAsync()
        {
            var reviews = await dbContext.Reviews
                .Include(r => r.Users)
                .Include(r => r.Books)
                .ThenInclude(c => c.CoverImage)
                .ToListAsync();

            return new OkObjectResult(reviews);
        }

        public async Task<IActionResult> GetReviewByIdAsync(Guid id)
        {
            var review = await dbContext.Reviews
                .Include(r => r.Users)
                .Include(r => r.Books)
                .ThenInclude(c => c.CoverImage)
                .FirstOrDefaultAsync(r => r.Id == id);
            if (review == null)
                return new NotFoundObjectResult(new
                {
                    message = "Review not found."
                });
            return new OkObjectResult(review);
        }

        public async Task<IActionResult> GetReviewsByBookAsync(Guid bookId)
        {
            var book = await dbContext.Books.FindAsync(bookId);
            if (book == null)
                return new NotFoundObjectResult(new
                {
                    message = "Book not found."
                });

            var reviews = await dbContext.Reviews
                .Where(r => r.BookId == bookId)
                .Include(r => r.Users)
                .ToListAsync();

            return new OkObjectResult(reviews);
        }

        public async Task<List<Reviews>> GetByUserIdAsync(Guid userId)
        {
            return await dbContext.Reviews
                .Include(b => b.Books)
                .ThenInclude(c => c.CoverImage)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

        public async Task<IActionResult> UpdateReviewAsync(Guid userId, Guid id, ReviewUpdateDto dto)
        {
            var review = await dbContext.Reviews.FindAsync(id);
            if (review == null)
                return new NotFoundObjectResult(new
                {
                    message = "Review not found."
                });

            if (review.UserId != userId)
                return new UnauthorizedObjectResult(new
                {
                    message = "You can only update your own reviews."
                });


            if (dto.BookId != null)
            {
                var bookExists = await dbContext.Books.AnyAsync(b => b.Id == dto.BookId);
                if (!bookExists) return new NotFoundObjectResult(new
                {
                    message = "Book not found."
                });
                review.BookId = dto.BookId.Value;
            }

            var oldBookId = review.BookId;

            if (dto.Rating != null) review.Rating = dto.Rating.Value;
            if (dto.Message != null) review.Message = dto.Message;

            review.UpdatedAt = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();

            if (dto.BookId != null && dto.BookId != oldBookId)
            {
                // Update both old and new books
                await UpdateAverageRatingAsync(oldBookId);
                await UpdateAverageRatingAsync(dto.BookId.Value);
            }
            else if (dto.Rating != null)
            {
                // Update only current book if rating changed
                await UpdateAverageRatingAsync(review.BookId);
            }

            return new OkObjectResult(review);
        }

        public async Task<IActionResult> DeleteReviewAsync(Guid userId, Guid id)
        {
            var review = await dbContext.Reviews.FindAsync(id);
            if (review == null)
                return new NotFoundObjectResult(new
                {
                    message = "Review not found."
                });

            if (review.UserId != userId)
                return new UnauthorizedObjectResult(new
                {
                    message = "You can only delete your own reviews."
                });

            dbContext.Reviews.Remove(review);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult(new
            {
                message = "Review deleted successfully."
            });
        }

        private async Task UpdateAverageRatingAsync(Guid bookId)
        {
            var reviews = await dbContext.Reviews
                .Where(r => r.BookId == bookId)
                .ToListAsync();

            var book = await dbContext.Books.FindAsync(bookId);
            if (book == null) return;

            if (reviews.Any())
            {
                var averageRating = Math.Round(reviews.Average(r => r.Rating), 2);
                book.AverageRating = (decimal)averageRating;
            }
            else
            {
                book.AverageRating = 0;
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
