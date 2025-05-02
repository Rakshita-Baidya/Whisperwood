using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Models;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly WhisperwoodDbContext dbContext;

        public GenreController(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddGenre(GenreDto dto)
        {
            var genre = new Genres
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description
            };

            dbContext.Genres.Add(genre);
            await dbContext.SaveChangesAsync();
            return Ok(genre);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllGenres()
        {
            var genres = await dbContext.Genres.ToListAsync();
            return Ok(genres);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetGenreById(Guid id)
        {
            var genre = await dbContext.Genres.FirstOrDefaultAsync(g => g.Id == id);
            return genre != null ? Ok(genre) : NotFound("Genre not found!");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateGenre(Guid id, GenreUpdateDto dto)
        {
            var genre = await dbContext.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound("Genre not found!");
            }

            if (dto.Name != null) genre.Name = dto.Name;
            if (dto.Description != null) genre.Description = dto.Description;

            await dbContext.SaveChangesAsync();
            return Ok(genre);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteGenre(Guid id)
        {
            var genre = await dbContext.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound("Genre not found!");
            }

            dbContext.Genres.Remove(genre);
            await dbContext.SaveChangesAsync();
            return Ok(new { Message = "Deleted successfully" });
        }
    }
}
