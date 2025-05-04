using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface ICategoryService
    {
        Task<IActionResult> AddCategoryAsync(Guid userId, CategoryDto dto);
        Task<IActionResult> GetAllCategoriesAsync();
        Task<IActionResult> GetCategoryByIdAsync(Guid userId, Guid id);
        Task<IActionResult> UpdateCategoryAsync(Guid userId, Guid id, CategoryUpdateDto dto);
        Task<IActionResult> DeleteCategoryAsync(Guid userId, Guid id);
    }
}
