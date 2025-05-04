using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IGenreService
    {
        Task<IActionResult> AddGenre(Guid userId, GenreDto dto);
        Task<IActionResult> GetAllGenres();
        Task<IActionResult> GetGenreById(Guid userId, Guid id);
        Task<IActionResult> UpdateGenre(Guid userId, Guid id, GenreUpdateDto dto);
        Task<IActionResult> DeleteGenre(Guid userId, Guid id);
    }
}
