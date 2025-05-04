using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IBookService
    {
        Task<IActionResult> AddBook(Guid userId, BookDto dto);
        Task<IActionResult> GetAllBooks();
        Task<IActionResult> GetBookById(Guid id);
        Task<IActionResult> UpdateBook(Guid userId, Guid id, BookUpdateDto dto);
        Task<IActionResult> DeleteBook(Guid userId, Guid id);
    }
}
