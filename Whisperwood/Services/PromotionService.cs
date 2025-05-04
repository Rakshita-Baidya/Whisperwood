using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;
using Whisperwood.Models;

namespace Whisperwood.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly WhisperwoodDbContext dbContext;

        public PromotionService(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> AddPromotion(Guid userId, PromotionDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return new BadRequestObjectResult("User not found. Are you logged in?");
            }

            if (!user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can create promotions.");
            }

            if (dto.StartDate > dto.EndDate)
            {
                return new BadRequestObjectResult("StartDate cannot be after EndDate.");
            }

            var promotion = new Promotions
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                DiscountPercent = dto.DiscountPercent,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                UserId = userId,
                User = user
            };

            dbContext.Promotions.Add(promotion);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult(promotion);
        }

        public async Task<IActionResult> GetAllPromotions()
        {
            var promotions = await dbContext.Promotions.Include(p => p.User).ToListAsync();
            return new OkObjectResult(promotions);
        }

        public async Task<IActionResult> GetPromotionById(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can view promotions by ID.");
            }

            var promotion = await dbContext.Promotions.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
            return promotion != null ? new OkObjectResult(promotion) : new NotFoundObjectResult("Promotion not found!");
        }

        public async Task<IActionResult> UpdatePromotion(Guid userId, Guid id, PromotionUpdateDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can update promotions.");
            }

            var promotion = await dbContext.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return new NotFoundObjectResult("Promotion not found!");
            }

            if (dto.StartDate != null && dto.EndDate != null && dto.StartDate > dto.EndDate)
            {
                return new BadRequestObjectResult("StartDate cannot be after EndDate.");
            }

            if (dto.Name != null) promotion.Name = dto.Name;
            if (dto.Description != null) promotion.Description = dto.Description;
            if (dto.DiscountPercent.HasValue) promotion.DiscountPercent = dto.DiscountPercent.Value;
            if (dto.StartDate != null) promotion.StartDate = (DateOnly)dto.StartDate;
            if (dto.EndDate != null) promotion.EndDate = (DateOnly)dto.EndDate;

            await dbContext.SaveChangesAsync();
            return new OkObjectResult(promotion);
        }

        public async Task<IActionResult> DeletePromotion(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can delete promotions.");
            }

            var promotion = await dbContext.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return new NotFoundObjectResult("Promotion not found!");
            }

            dbContext.Promotions.Remove(promotion);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult("Promotion deleted successfully");
        }
    }
}
