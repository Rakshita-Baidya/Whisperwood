using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IBookService
    {
        Task<IActionResult> AddBookAsync(Guid userId, BookDto dto);
        Task<IActionResult> GetAllBooksAsync();
        Task<IActionResult> GetBookByIdAsync(Guid id);
        Task<IActionResult> UpdateBookAsync(Guid userId, Guid id, BookUpdateDto dto);
        Task<IActionResult> DeleteBookAsync(Guid userId, Guid id);

        Task<IActionResult> GetFilteredBooksAsync(BookFilterDto filter);
    }
}
