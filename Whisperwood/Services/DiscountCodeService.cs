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

        public async Task<IActionResult> AddDiscountCode(Guid userId, DiscountCodeDto dto)
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

        public async Task<IActionResult> GetAllDiscountCodes()
        {
            var discountCodeList = await dbContext.DiscountCodes.ToListAsync();
            return new OkObjectResult(discountCodeList);
        }

        public async Task<IActionResult> GetDiscountCodeById(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can view discount codes.");
            }

            var discountCode = await dbContext.DiscountCodes.FirstOrDefaultAsync(d => d.Id == id);
            return discountCode != null ? new OkObjectResult(discountCode) : new NotFoundObjectResult("Discount code not found!");
        }

        public async Task<IActionResult> UpdateDiscountCode(Guid userId, Guid id, DiscountCodeUpdateDto dto)
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

        public async Task<IActionResult> DeleteDiscountCode(Guid userId, Guid id)
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
