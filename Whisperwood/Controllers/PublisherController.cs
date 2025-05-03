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
    public class PublisherController : BaseController
    {
        private readonly WhisperwoodDbContext dbContext;

        public PublisherController(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddPublisher(PublisherDto dto)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
            }
            var publisher = new Publishers
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                Address = dto.Address,
                Contact = dto.Contact
            };

            dbContext.Publishers.Add(publisher);
            await dbContext.SaveChangesAsync();
            return Ok(publisher);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllPublishers()
        {
            List<Publishers> publisherList = await dbContext.Publishers.ToListAsync();
            return Ok(publisherList);
        }

        [HttpGet("getbyid/{id}")]
        [Authorize]
        public async Task<IActionResult> GetPublisherById(Guid id)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
            }
            Publishers? publisher = await dbContext.Publishers.FirstOrDefaultAsync(p => p.Id == id);
            return publisher != null ? Ok(publisher) : NotFound("Publisher not found!");
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePublisher(Guid id, PublisherUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
            }
            var publisher = await dbContext.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return NotFound("Publisher not found!");
            }

            if (dto.Name != null) publisher.Name = dto.Name;
            if (dto.Email != null) publisher.Email = dto.Email;
            if (dto.Address != null) publisher.Address = dto.Address;
            if (dto.Contact != null) publisher.Contact = dto.Contact;

            await dbContext.SaveChangesAsync();
            return Ok(publisher);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePublisher(Guid id)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
            }
            var publisher = await dbContext.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return NotFound("Publisher not found!");
            }

            dbContext.Publishers.Remove(publisher);
            await dbContext.SaveChangesAsync();
            return Ok("Deleted successfully");
        }

    }
}
