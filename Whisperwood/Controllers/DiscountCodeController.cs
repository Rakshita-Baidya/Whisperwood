using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Models;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountCodeController : BaseController
    {
        private readonly WhisperwoodDbContext dbContext;

        public DiscountCodeController(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddDiscountCode(DiscountCodeDto dto)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
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
            return Ok(discountCode);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllDiscountCodes()
        {
            List<DiscountCode> discountCodeList = await dbContext.DiscountCodes.ToListAsync();
            return Ok(discountCodeList);
        }

        [HttpGet("getbyid/{id}")]
        [Authorize]
        public async Task<IActionResult> GetDiscountCodeById(Guid id)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
            }
            DiscountCode? discountCode = await dbContext.DiscountCodes.FirstOrDefaultAsync(d => d.Id == id);
            return discountCode != null ? Ok(discountCode) : NotFound("Discount code not found!");
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateDiscountCode(Guid id, DiscountCodeUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
            }
            var discountCode = await dbContext.DiscountCodes.FindAsync(id);
            if (discountCode == null)
            {
                return NotFound("Discount code not found!");
            }

            if (dto.Percent != null) discountCode.Percent = dto.Percent.Value;
            if (dto.StartDate != null) discountCode.StartDate = dto.StartDate.Value;
            if (dto.EndDate != null) discountCode.EndDate = dto.EndDate.Value;

            await dbContext.SaveChangesAsync();
            return Ok(discountCode);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteDiscountCode(Guid id)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
            }
            var discountCode = await dbContext.DiscountCodes.FindAsync(id);
            if (discountCode == null)
            {
                return NotFound("Discount code not found!");
            }
            dbContext.DiscountCodes.Remove(discountCode);
            await dbContext.SaveChangesAsync();
            return Ok("Deleted successfully!");
        }
    }
}
