using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IAuthorService
    {
        Task<IActionResult> AddAuthorAsync(Guid userId, AuthorDto dto);
        Task<IActionResult> GetAllAuthorsAsync();
        Task<IActionResult> GetAuthorByIdAsync(Guid userId, Guid id);
        Task<IActionResult> UpdateAuthorAsync(Guid userId, Guid id, AuthorUpdateDto dto);
        Task<IActionResult> DeleteAuthorAsync(Guid userId, Guid id);
    }
}
