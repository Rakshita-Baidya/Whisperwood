using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Models;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly WhisperwoodDbContext dbContext;

        public CategoryController(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddCategory(CategoryDto dto)
        {
            var category = new Categories
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description
            };

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();
            return Ok(category);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await dbContext.Categories.ToListAsync();
            return Ok(categories);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            return category != null ? Ok(category) : NotFound("Category not found!");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, CategoryUpdateDto dto)
        {
            var category = await dbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound("Category not found!");
            }

            if (dto.Name != null) category.Name = dto.Name;
            if (dto.Description != null) category.Description = dto.Description;

            await dbContext.SaveChangesAsync();
            return Ok(category);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            var category = await dbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound("Category not found!");
            }

            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();
            return Ok(new { Message = "Deleted successfully" });
        }
    }
}
