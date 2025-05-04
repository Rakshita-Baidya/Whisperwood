using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface ICategoryService
    {
        Task<IActionResult> AddCategory(Guid userId, CategoryDto dto);
        Task<IActionResult> GetAllCategories();
        Task<IActionResult> GetCategoryById(Guid userId, Guid id);
        Task<IActionResult> UpdateCategory(Guid userId, Guid id, CategoryUpdateDto dto);
        Task<IActionResult> DeleteCategory(Guid userId, Guid id);
    }
}
