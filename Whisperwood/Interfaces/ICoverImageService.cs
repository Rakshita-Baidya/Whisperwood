using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface ICoverImageService
    {
        Task<IActionResult> AddCoverImageAsync(Guid userId, CoverImageDto dto);
        Task<IActionResult> GetAllCoverImagesAsync();
        Task<IActionResult> GetCoverImageByIdAsync(Guid userId, Guid id);
        Task<IActionResult> UpdateCoverImageAsync(Guid userId, Guid id, CoverImageUpdateDto dto);
        Task<IActionResult> DeleteCoverImageAsync(Guid userId, Guid id);
    }
}
