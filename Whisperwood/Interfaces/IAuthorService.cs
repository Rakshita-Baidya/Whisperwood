using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IAuthorService
    {
        Task<IActionResult> AddAuthor(Guid userId, AuthorDto dto);
        Task<IActionResult> GetAllAuthors();
        Task<IActionResult> GetAuthorById(Guid userId, Guid id);
        Task<IActionResult> UpdateAuthor(Guid userId, Guid id, AuthorUpdateDto dto);
        Task<IActionResult> DeleteAuthor(Guid userId, Guid id);
    }
}
