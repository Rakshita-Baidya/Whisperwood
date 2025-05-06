using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;
using Whisperwood.Models;

namespace Whisperwood.Services
{
    public class DiscountCodeService : IDiscountCodeService
    {
        private readonly WhisperwoodDbContext dbContext;

        public DiscountCodeService(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public decimal CalculateDiscount(decimal subTotal, int totalItems, int userOrdersCount, Promotions? promotion)
        {
            decimal currentSubTotal = subTotal;
            decimal totalDiscount = 0m;

            if (promotion != null)
            {
                decimal promoDiscount = currentSubTotal * promotion.DiscountPercent / 100;
                totalDiscount += promoDiscount;
                currentSubTotal -= promoDiscount;
            }

            if (totalItems >= 5)
            {
                decimal fivePercentDiscount = currentSubTotal * 0.05m;
                totalDiscount += fivePercentDiscount;
                currentSubTotal -= fivePercentDiscount;
            }

            if (userOrdersCount >= 10)
            {
                decimal tenPercentDiscount = currentSubTotal * 0.10m;
                totalDiscount += tenPercentDiscount;
            }

            return totalDiscount;
        }

        public decimal GetPromotionDiscount(decimal subTotal, Promotions? promotion)
        {
            return promotion != null ? subTotal * promotion.DiscountPercent : 0m;
        }

        public async Task<IActionResult> AddDiscountCodeAsync(Guid userId, DiscountCodeDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can add discount codes.");
            }

            var discountCode = new DiscountCode
            {
                Id = Guid.NewGuid(),
                Code = Guid.NewGuid().ToString().Substring(0, 8),
                Percent = dto.Percent,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
            };

            dbContext.DiscountCodes.Add(discountCode);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult(discountCode);
        }

        public async Task<IActionResult> GetAllDiscountCodesAsync()
        {
            var discountCodeList = await dbContext.DiscountCodes.ToListAsync();
            return new OkObjectResult(discountCodeList);
        }

        public async Task<IActionResult> GetDiscountCodeByIdAsync(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can view discount codes.");
            }

            var discountCode = await dbContext.DiscountCodes.FirstOrDefaultAsync(d => d.Id == id);
            return discountCode != null ? new OkObjectResult(discountCode) : new NotFoundObjectResult("Discount code not found!");
        }

        public async Task<IActionResult> UpdateDiscountCodeAsync(Guid userId, Guid id, DiscountCodeUpdateDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can update discount codes.");
            }

            var discountCode = await dbContext.DiscountCodes.FindAsync(id);
            if (discountCode == null)
            {
                return new NotFoundObjectResult("Discount code not found!");
            }

            if (dto.Percent != null) discountCode.Percent = dto.Percent.Value;
            if (dto.StartDate != null) discountCode.StartDate = dto.StartDate.Value;
            if (dto.EndDate != null) discountCode.EndDate = dto.EndDate.Value;

            await dbContext.SaveChangesAsync();
            return new OkObjectResult(discountCode);
        }

        public async Task<IActionResult> DeleteDiscountCodeAsync(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can delete discount codes.");
            }

            var discountCode = await dbContext.DiscountCodes.FindAsync(id);
            if (discountCode == null)
            {
                return new NotFoundObjectResult("Discount code not found!");
            }

            dbContext.DiscountCodes.Remove(discountCode);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult("Deleted successfully!");
        }
    }
}
