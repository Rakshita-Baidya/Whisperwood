using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IReviewService
    {
        Task<IActionResult> AddReviewAsync(Guid userId, ReviewDTO dto);
        Task<IActionResult> GetAllReviewsAsync();
        Task<IActionResult> GetReviewsByBookAsync(Guid bookId);
        Task<IActionResult> UpdateReviewAsync(Guid userId, Guid id, ReviewUpdateDto dto);
        Task<IActionResult> DeleteReviewAsync(Guid userId, Guid id);
    }
}
