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

        public async Task<IActionResult> AddPromotionAsync(Guid userId, PromotionDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                if (!user.IsAdmin.GetValueOrDefault(false) && !user.IsStaff.GetValueOrDefault(false))
                {
                    return new UnauthorizedObjectResult(new
                    {
                        message = "Only admins or staff can add promotions."
                    });
                }
            }

            if (dto.StartDate > dto.EndDate)
            {
                return new BadRequestObjectResult(new
                {
                    message = "StartDate cannot be after EndDate."
                });
            }

            var promotion = new Promotions
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description,
                Code = Guid.NewGuid().ToString().Substring(0, 8),
                DiscountPercent = dto.DiscountPercent,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                UserId = userId,
                User = user
            };
            RecalculateIsActive(promotion);
            dbContext.Promotions.Add(promotion);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult(promotion);
        }

        public async Task<IActionResult> GetAllPromotionsAsync()
        {
            var promotions = await dbContext.Promotions.Include(p => p.User).ToListAsync();
            bool hasChanges = false;

            foreach (var promotion in promotions)
            {
                bool previousState = promotion.IsActive;
                RecalculateIsActive(promotion);
                if (promotion.IsActive != previousState)
                {
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                await dbContext.SaveChangesAsync();
            }

            return new OkObjectResult(promotions);
        }


        public async Task<IActionResult> GetPromotionByIdAsync(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            var promotion = await dbContext.Promotions.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);

            if (promotion == null)
            {
                return new NotFoundObjectResult(new
                {
                    message = "Promotion not found!"
                });
            }

            var previousState = promotion.IsActive;
            RecalculateIsActive(promotion);
            if (promotion.IsActive != previousState)
            {
                await dbContext.SaveChangesAsync();
            }

            return new OkObjectResult(promotion);
        }


        public async Task<IActionResult> UpdatePromotionAsync(Guid userId, Guid id, PromotionUpdateDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                if (!user.IsAdmin.GetValueOrDefault(false) && !user.IsStaff.GetValueOrDefault(false))
                {
                    return new UnauthorizedObjectResult(new
                    {
                        message = "Only admins or staff can update promotions."
                    });
                }
            }

            var promotion = await dbContext.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return new NotFoundObjectResult(new
                {
                    message = "Promotion not found!"
                });
            }

            if (dto.StartDate != null && dto.EndDate != null && dto.StartDate > dto.EndDate)
            {
                return new BadRequestObjectResult(new
                {
                    message = "StartDate cannot be after EndDate."
                });
            }

            if (dto.Name != null) promotion.Name = dto.Name;
            if (dto.Description != null) promotion.Description = dto.Description;
            if (dto.DiscountPercent.HasValue) promotion.DiscountPercent = dto.DiscountPercent.Value;
            if (dto.StartDate != null) promotion.StartDate = (DateOnly)dto.StartDate;
            if (dto.EndDate != null) promotion.EndDate = (DateOnly)dto.EndDate;

            RecalculateIsActive(promotion);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult(promotion);
        }

        public async Task<IActionResult> DeletePromotionAsync(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                if (!user.IsAdmin.GetValueOrDefault(false) && !user.IsStaff.GetValueOrDefault(false))
                {
                    return new UnauthorizedObjectResult(new
                    {
                        message = "Only admins or staff can delete promotions."
                    });
                }
            }

            var promotion = await dbContext.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return new NotFoundObjectResult(new
                {
                    message = "Promotion not found!"
                });
            }

            dbContext.Promotions.Remove(promotion);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult(new
            {
                message = "Promotion deleted successfully"
            });
        }

        private void RecalculateIsActive(Promotions promotion)
        {
            var currentDate = DateOnly.FromDateTime(DateTime.Now);
            promotion.IsActive = promotion.StartDate <= currentDate && currentDate <= promotion.EndDate;
        }


        public async Task<(Promotions? Promotion, string? Error)> ValidatePromoCodeAsync(Guid userId, string? promoCode)
        {
            if (string.IsNullOrEmpty(promoCode))
                return (null, null);

            var currentDate = DateOnly.FromDateTime(DateTime.Now);

            var promotion = await dbContext.Promotions
                .FirstOrDefaultAsync(p => p.Code == promoCode && p.IsActive &&
                                        p.StartDate <= currentDate && p.EndDate >= currentDate);

            if (promotion == null)
                return (null, "Invalid or expired promo code.");

            var promoUsed = await dbContext.Orders
                .AnyAsync(o => o.UserId == userId && o.PromoCode == promoCode &&
                              o.Status != Orders.OrderStatus.Cancelled);

            if (promoUsed)
                return (null, "This promo code has already been used.");

            return (promotion, null);
        }


    }
}
