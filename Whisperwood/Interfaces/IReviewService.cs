using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IReviewService
    {
        Task<IActionResult> AddReview(Guid userId, ReviewDTO dto);
        Task<IActionResult> GetAllReviews();
        Task<IActionResult> GetReviewsByBook(Guid bookId);
        Task<IActionResult> UpdateReview(Guid userId, Guid id, ReviewUpdateDto dto);
        Task<IActionResult> DeleteReview(Guid userId, Guid id);
    }
}
