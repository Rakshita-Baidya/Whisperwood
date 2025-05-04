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
    public class PromotionController : BaseController
    {
        private readonly WhisperwoodDbContext dbContext;

        public PromotionController(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddPromotion(PromotionDto dto)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return BadRequest("User not found. Are you logged in?");
            }

            if (!user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can create promotions.");
            }

            if (dto.StartDate > dto.EndDate)
            {
                return BadRequest("StartDate cannot be after EndDate.");
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
            return Ok(promotion);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllPromotions()
        {
            var promotions = await dbContext.Promotions.Include(p => p.User).ToListAsync();
            return Ok(promotions);
        }

        [HttpGet("getbyid/{id}")]
        [Authorize]
        public async Task<IActionResult> GetPromotionById(Guid id)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can view promotions by ID.");
            }

            var promotion = await dbContext.Promotions.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
            return promotion != null ? Ok(promotion) : NotFound("Promotion not found!");
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePromotion(Guid id, PromotionUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update promotions.");
            }

            var promotion = await dbContext.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound("Promotion not found!");
            }

            if (dto.StartDate > dto.EndDate)
            {
                return BadRequest("StartDate cannot be after EndDate.");
            }

            if (dto.Name != null) promotion.Name = dto.Name;
            if (dto.Description != null) promotion.Description = dto.Description;
            if (dto.DiscountPercent.HasValue) promotion.DiscountPercent = dto.DiscountPercent.Value;
            if (dto.StartDate != null) promotion.StartDate = (DateOnly)dto.StartDate;
            if (dto.EndDate != null) promotion.EndDate = (DateOnly)dto.EndDate;

            await dbContext.SaveChangesAsync();
            return Ok(promotion);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePromotion(Guid id)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can delete promotions.");
            }

            var promotion = await dbContext.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return NotFound("Promotion not found!");
            }

            dbContext.Promotions.Remove(promotion);
            await dbContext.SaveChangesAsync();
            return Ok("Promotion deleted successfully");
        }
    }
}
