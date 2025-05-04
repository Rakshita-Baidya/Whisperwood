using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IGenreService
    {
        Task<IActionResult> AddGenreAsync(Guid userId, GenreDto dto);
        Task<IActionResult> GetAllGenresAsync();
        Task<IActionResult> GetGenreByIdAsync(Guid userId, Guid id);
        Task<IActionResult> UpdateGenreAsync(Guid userId, Guid id, GenreUpdateDto dto);
        Task<IActionResult> DeleteGenreAsync(Guid userId, Guid id);
    }
}
