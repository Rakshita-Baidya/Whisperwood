using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Models;

namespace Whisperwood.Interfaces
{
    public interface IReviewService
    {
        Task<IActionResult> AddReviewAsync(Guid userId, ReviewDTO dto);
        Task<IActionResult> GetAllReviewsAsync();
        Task<IActionResult> GetReviewByIdAsync(Guid id);
        Task<IActionResult> GetReviewsByBookAsync(Guid bookId);
        Task<IActionResult> UpdateReviewAsync(Guid userId, Guid id, ReviewUpdateDto dto);
        Task<IActionResult> DeleteReviewAsync(Guid userId, Guid id);
        Task<List<Reviews>> GetByUserIdAsync(Guid userId);
    }
}
