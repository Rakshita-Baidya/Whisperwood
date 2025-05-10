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

        public decimal CalculateDiscount(decimal subTotal, int totalItems, int userOrdersCount, Promotions appliedPromotion)
        {
            decimal currentSubTotal = subTotal;

            decimal promoDiscount = GetPromotionDiscount(currentSubTotal, appliedPromotion);
            currentSubTotal -= promoDiscount;

            decimal bulkDiscount = GetBulkDiscount(currentSubTotal, totalItems);
            currentSubTotal -= bulkDiscount;

            decimal loyalDiscount = GetLoyalDiscount(currentSubTotal, userOrdersCount);

            return promoDiscount + bulkDiscount + loyalDiscount;
        }

        public decimal GetPromotionDiscount(decimal subTotal, Promotions appliedPromotion)
        {
            if (appliedPromotion == null || appliedPromotion.DiscountPercent <= 0)
                return 0;

            return subTotal * (appliedPromotion.DiscountPercent / 100);
        }

        public decimal GetBulkDiscount(decimal subTotal, int totalItems)
        {
            if (totalItems >= 5)
                return subTotal * 0.05m;
            return 0;
        }

        public decimal GetLoyalDiscount(decimal subTotal, int userOrdersCount)
        {
            if (userOrdersCount >= 10)
                return subTotal * 0.1m;
            return 0;
        }

        public async Task<IActionResult> AddDiscountCodeAsync(Guid userId, DiscountCodeDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult(new { message = "Only admins can add discount codes." });
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
                return new UnauthorizedObjectResult(new
                {
                    message = "Only admins can view discount codes."
                });
            }

            var discountCode = await dbContext.DiscountCodes.FirstOrDefaultAsync(d => d.Id == id);
            return discountCode != null ? new OkObjectResult(discountCode) : new NotFoundObjectResult(new
            {
                message = "Discount code not found!"
            });
        }

        public async Task<IActionResult> UpdateDiscountCodeAsync(Guid userId, Guid id, DiscountCodeUpdateDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult(new
                {
                    message = "Only admins can update discount codes."
                });
            }

            var discountCode = await dbContext.DiscountCodes.FindAsync(id);
            if (discountCode == null)
            {
                return new NotFoundObjectResult(new
                {
                    message = "Discount code not found!"
                });
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
                return new UnauthorizedObjectResult(new
                {
                    message = "Only admins can delete discount codes."
                });
            }

            var discountCode = await dbContext.DiscountCodes.FindAsync(id);
            if (discountCode == null)
            {
                return new NotFoundObjectResult(new
                {
                    message = "Discount code not found!"
                });
            }

            dbContext.DiscountCodes.Remove(discountCode);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult(new
            {
                message = "Deleted successfully!"
            });
        }
    }
}
