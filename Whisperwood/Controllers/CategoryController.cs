using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : BaseController
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddCategory(CategoryDto dto)
        {
            var userId = GetLoggedInUserId();
            return await categoryService.AddCategory(userId, dto);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllCategories()
        {
            return await categoryService.GetAllCategories();
        }

        [HttpGet("getbyid/{id}")]
        [Authorize]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await categoryService.GetCategoryById(userId, id);
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory(Guid id, CategoryUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            return await categoryService.UpdateCategory(userId, id, dto);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await categoryService.DeleteCategory(userId, id);
        }
    }
}
