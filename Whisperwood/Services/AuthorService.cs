using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;
using Whisperwood.Models;

namespace Whisperwood.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly WhisperwoodDbContext dbContext;

        public AuthorService(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> AddAuthor(Guid userId, AuthorDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can add authors.");
            }

            var author = new Authors
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Address = dto.Address,
                Nationality = dto.Nationality,
                DOB = dto.DOB,
                Contact = dto.Contact,
            };

            dbContext.Authors.Add(author);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult(author);
        }

        public async Task<IActionResult> GetAllAuthors()
        {
            var authorList = await dbContext.Authors.ToListAsync();
            return new OkObjectResult(authorList);
        }

        public async Task<IActionResult> GetAuthorById(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can view authors.");
            }

            var author = await dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);
            return author != null ? new OkObjectResult(author) : new NotFoundObjectResult("Author not found!");
        }

        public async Task<IActionResult> UpdateAuthor(Guid userId, Guid id, AuthorUpdateDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can update authors.");
            }

            var author = await dbContext.Authors.FindAsync(id);
            if (author == null)
            {
                return new NotFoundObjectResult("Author not found!");
            }

            if (dto.Name != null) author.Name = dto.Name;
            if (dto.Email != null) author.Email = dto.Email;
            if (dto.Address != null) author.Address = dto.Address;
            if (dto.Nationality != null) author.Nationality = dto.Nationality;
            if (dto.DOB.HasValue) author.DOB = dto.DOB.Value;
            if (dto.Contact != null) author.Contact = dto.Contact;

            await dbContext.SaveChangesAsync();
            return new OkObjectResult(author);
        }

        public async Task<IActionResult> DeleteAuthor(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can delete authors.");
            }

            var author = await dbContext.Authors.FindAsync(id);
            if (author == null)
            {
                return new NotFoundObjectResult("Author not found!");
            }

            dbContext.Authors.Remove(author);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult("Deleted successfully");
        }
    }
}
