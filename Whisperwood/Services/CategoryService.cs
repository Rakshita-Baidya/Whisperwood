using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;
using Whisperwood.Models;

namespace Whisperwood.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly WhisperwoodDbContext dbContext;

        public CategoryService(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> AddCategoryAsync(Guid userId, CategoryDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can add categories.");
            }

            var category = new Categories
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description ?? ""
            };

            dbContext.Categories.Add(category);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult(category);
        }

        public async Task<IActionResult> GetAllCategoriesAsync()
        {
            var categories = await dbContext.Categories.ToListAsync();
            return new OkObjectResult(categories);
        }

        public async Task<IActionResult> GetCategoryByIdAsync(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can view categories.");
            }

            var category = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id);
            return category != null ? new OkObjectResult(category) : new NotFoundObjectResult("Category not found!");
        }

        public async Task<IActionResult> UpdateCategoryAsync(Guid userId, Guid id, CategoryUpdateDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can update categories.");
            }

            var category = await dbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return new NotFoundObjectResult("Category not found!");
            }

            if (dto.Name != null) category.Name = dto.Name;
            if (dto.Description != null) category.Description = dto.Description;

            await dbContext.SaveChangesAsync();
            return new OkObjectResult(category);
        }

        public async Task<IActionResult> DeleteCategoryAsync(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can delete categories.");
            }

            var category = await dbContext.Categories.FindAsync(id);
            if (category == null)
            {
                return new NotFoundObjectResult("Category not found!");
            }

            dbContext.Categories.Remove(category);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult("Deleted successfully");
        }
    }
}
