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
    public class GenreController : BaseController
    {
        private readonly WhisperwoodDbContext dbContext;

        public GenreController(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddGenre(GenreDto dto)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
            }
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
        [Authorize]
        public async Task<IActionResult> GetGenreById(Guid id)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
            }
            var genre = await dbContext.Genres.FirstOrDefaultAsync(g => g.Id == id);
            return genre != null ? Ok(genre) : NotFound("Genre not found!");
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateGenre(Guid id, GenreUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
            }
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
        [Authorize]
        public async Task<IActionResult> DeleteGenre(Guid id)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
            }
            var genre = await dbContext.Genres.FindAsync(id);
            if (genre == null)
            {
                return NotFound("Genre not found!");
            }

            dbContext.Genres.Remove(genre);
            await dbContext.SaveChangesAsync();
            return Ok("Deleted successfully");
        }
    }
}
