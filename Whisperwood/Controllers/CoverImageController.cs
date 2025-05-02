using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Models;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoverImageController : ControllerBase
    {
        private readonly WhisperwoodDbContext dbContext;

        public CoverImageController(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddCoverImage(CoverImageDto dto)
        {
            var coverImage = new CoverImages
            {
                Id = Guid.NewGuid(),
                CoverImageURL = dto.CoverImageURL
            };
            dbContext.CoverImages.Add(coverImage);
            await dbContext.SaveChangesAsync();
            return Ok(coverImage);

        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllCoverImages()
        {
            List<CoverImages> coverImageList = await dbContext.CoverImages.ToListAsync();
            return Ok(coverImageList);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetCoverImageById(Guid id)
        {
            CoverImages? coverImage = await dbContext.CoverImages.FirstOrDefaultAsync(c => c.Id == id);
            return coverImage != null ? Ok(coverImage) : NotFound("Cover Image not found! Please check the id again.");
        }

        [HttpPut("updateCoverImage/{id}")]
        public async Task<IActionResult> UpdateCoverImage(Guid id, CoverImageUpdateDto dto)
        {
            var coverImage = await dbContext.CoverImages.FindAsync(id);
            if (coverImage == null)
            {
                return NotFound("Author not found");
            }

            if (dto.CoverImageURL != null) coverImage.CoverImageURL = dto.CoverImageURL;

            await dbContext.SaveChangesAsync();
            return Ok(coverImage);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCoverImage(Guid id)
        {
            var coverImage = await dbContext.CoverImages.FindAsync(id);
            if (coverImage == null)
            {
                return NotFound("Cover Image not found!");
            }
            dbContext.CoverImages.Remove(coverImage);
            await dbContext.SaveChangesAsync();
            return Ok(new { Message = "Cover Image deleted successfully!" });
        }

    }
}
