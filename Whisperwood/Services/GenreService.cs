using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;
using Whisperwood.Models;

namespace Whisperwood.Services
{
    public class GenreService : IGenreService
    {
        private readonly WhisperwoodDbContext dbContext;

        public GenreService(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> AddGenreAsync(Guid userId, GenreDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                if (!user.IsAdmin.GetValueOrDefault(false) && !user.IsStaff.GetValueOrDefault(false))
                {
                    return new UnauthorizedObjectResult(new
                    {
                        message = "Only admins or staff can add genres."
                    });
                }
            }

            var genre = new Genres
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Description = dto.Description
            };

            dbContext.Genres.Add(genre);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult(genre);
        }

        public async Task<IActionResult> GetAllGenresAsync()
        {
            var genres = await dbContext.Genres.ToListAsync();
            return new OkObjectResult(genres);
        }

        public async Task<IActionResult> GetGenreByIdAsync(Guid id)
        {
            var genre = await dbContext.Genres.FirstOrDefaultAsync(g => g.Id == id);
            return genre != null ? new OkObjectResult(genre) : new NotFoundObjectResult(new
            {
                message = "Genre not found!"
            });
        }

        public async Task<IActionResult> UpdateGenreAsync(Guid userId, Guid id, GenreUpdateDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                if (!user.IsAdmin.GetValueOrDefault(false) && !user.IsStaff.GetValueOrDefault(false))
                {
                    return new UnauthorizedObjectResult(new
                    {
                        message = "Only admins or staff can update genres."
                    });
                }
            }
            var genre = await dbContext.Genres.FindAsync(id);
            if (genre == null)
            {
                return new NotFoundObjectResult(new
                {
                    message = "Genre not found!"
                });
            }

            if (dto.Name != null) genre.Name = dto.Name;
            if (dto.Description != null) genre.Description = dto.Description;

            await dbContext.SaveChangesAsync();
            return new OkObjectResult(genre);
        }

        public async Task<IActionResult> DeleteGenreAsync(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                if (!user.IsAdmin.GetValueOrDefault(false) && !user.IsStaff.GetValueOrDefault(false))
                {
                    return new UnauthorizedObjectResult(new
                    {
                        message = "Only admins or staff can delete genres."
                    });
                }
            }

            var genre = await dbContext.Genres.FindAsync(id);
            if (genre == null)
            {
                return new NotFoundObjectResult(new
                {
                    message = "Genre not found!"
                });
            }

            dbContext.Genres.Remove(genre);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult(new
            {
                message = "Deleted successfully"
            });
        }
    }
}
