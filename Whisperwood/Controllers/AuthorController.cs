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
    public class AuthorController : BaseController
    {
        private readonly WhisperwoodDbContext dbContext;

        public AuthorController(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddAuthor(AuthorDto dto)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
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
            return Ok(author);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllAuthors()
        {
            List<Authors> authorList = await dbContext.Authors.ToListAsync();
            return Ok(authorList);
        }

        [HttpGet("getbyid/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAuthorById(Guid id)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
            }
            Authors? author = await dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);
            return author != null ? Ok(author) : NotFound("Author not found!");
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAuthor(Guid id, AuthorUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
            }
            var author = await dbContext.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound("Author not found!");
            }

            if (dto.Name != null) author.Name = dto.Name;
            if (dto.Email != null) author.Email = dto.Email;
            if (dto.Address != null) author.Address = dto.Address;
            if (dto.Nationality != null) author.Nationality = dto.Nationality;
            if (dto.DOB.HasValue) author.DOB = dto.DOB.Value;
            if (dto.Contact != null) author.Contact = dto.Contact;

            await dbContext.SaveChangesAsync();
            return Ok(author);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAuthor(Guid id)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
            }
            var author = await dbContext.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound("Author not found!");
            }

            dbContext.Authors.Remove(author);
            await dbContext.SaveChangesAsync();
            return Ok("Deleted successfully");
        }
    }
}