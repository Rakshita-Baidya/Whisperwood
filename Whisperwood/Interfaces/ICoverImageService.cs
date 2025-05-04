using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface ICoverImageService
    {
        Task<IActionResult> AddCoverImage(Guid userId, CoverImageDto dto);
        Task<IActionResult> GetAllCoverImages();
        Task<IActionResult> GetCoverImageById(Guid userId, Guid id);
        Task<IActionResult> UpdateCoverImage(Guid userId, Guid id, CoverImageUpdateDto dto);
        Task<IActionResult> DeleteCoverImage(Guid userId, Guid id);
    }
}
