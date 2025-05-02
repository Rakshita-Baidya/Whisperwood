using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Models;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly WhisperwoodDbContext dbContext;

        public PublisherController(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddPublisher(PublisherDto dto)
        {
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
        public async Task<IActionResult> GetPublisherById(Guid id)
        {
            Publishers? publisher = await dbContext.Publishers.FirstOrDefaultAsync(p => p.Id == id);
            return publisher != null ? Ok(publisher) : NotFound("Publisher not found!");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdatePublisher(Guid id, PublisherUpdateDto dto)
        {
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
        public async Task<IActionResult> DeletePublisher(Guid id)
        {
            var publisher = await dbContext.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return NotFound("Publisher not found!");
            }

            dbContext.Publishers.Remove(publisher);
            await dbContext.SaveChangesAsync();
            return Ok(new { Message = "Deleted successfully" });
        }

    }
}
