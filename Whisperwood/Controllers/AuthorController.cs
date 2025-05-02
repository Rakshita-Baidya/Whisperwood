using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;
using Whisperwood.Models;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly WhisperwoodDbContext dbContext;
        private readonly IAuthService authService;

        public AuthorController(WhisperwoodDbContext dbContext, IAuthService authService)
        {
            this.dbContext = dbContext;
            this.authService = authService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddAuthor(AuthorDto dto)
        {
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
        [Authorize]
        public async Task<IActionResult> GetAllAuthors()
        {
            if (!await authService.IsAdminAsync(User))
                return Forbid("Only admins can add authors.");

            List<Authors> authorList = await dbContext.Authors.ToListAsync();
            return Ok(authorList);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetAuthorById(Guid id)
        {
            Authors? author = await dbContext.Authors.FirstOrDefaultAsync(a => a.Id == id);
            return author != null ? Ok(author) : NotFound("Author not found!");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAuthor(Guid id, AuthorUpdateDto dto)
        {
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
            return Ok(new
            {
                Id = author.Id,
                Author = new AuthorDto
                {
                    Name = author.Name!,
                    Email = author.Email!,
                    Address = author.Address,
                    Nationality = author.Nationality,
                    DOB = author.DOB,
                    Contact = author.Contact
                }
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAuthor(Guid id)
        {
            var author = await dbContext.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound("Author not found!");
            }

            dbContext.Authors.Remove(author);
            await dbContext.SaveChangesAsync();
            return Ok(new { Message = "Deleted successfully" });
        }
    }
}