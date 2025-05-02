using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Models;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountCodeController : ControllerBase
    {
        private readonly WhisperwoodDbContext dbContext;

        public DiscountCodeController(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddDiscountCode(DiscountCodeDto dto)
        {
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
        public async Task<IActionResult> GetDiscountCodeById(Guid id)
        {
            DiscountCode? discountCode = await dbContext.DiscountCodes.FirstOrDefaultAsync(d => d.Id == id);
            return discountCode != null ? Ok(discountCode) : NotFound("Discount code not found!");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateDiscountCode(Guid id, DiscountCodeUpdateDto dto)
        {
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
        public async Task<IActionResult> DeleteDiscountCode(Guid id)
        {
            var discountCode = await dbContext.DiscountCodes.FindAsync(id);
            if (discountCode == null)
            {
                return NotFound("Discount code not found!");
            }
            dbContext.DiscountCodes.Remove(discountCode);
            await dbContext.SaveChangesAsync();
            return Ok(new { Message = "Discount code deleted successfully!" });
        }
    }
}
