using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;
using Whisperwood.Models;

namespace Whisperwood.Services
{
    public class PublisherService : IPublisherService
    {
        private readonly WhisperwoodDbContext dbContext;

        public PublisherService(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> AddPublisher(Guid userId, PublisherDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can add publishers.");
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
            return new OkObjectResult(publisher);
        }

        public async Task<IActionResult> GetAllPublishers()
        {
            var publisherList = await dbContext.Publishers.ToListAsync();
            return new OkObjectResult(publisherList);
        }

        public async Task<IActionResult> GetPublisherById(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can view publishers.");
            }

            var publisher = await dbContext.Publishers.FirstOrDefaultAsync(p => p.Id == id);
            return publisher != null ? new OkObjectResult(publisher) : new NotFoundObjectResult("Publisher not found!");
        }

        public async Task<IActionResult> UpdatePublisher(Guid userId, Guid id, PublisherUpdateDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can update publishers.");
            }

            var publisher = await dbContext.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return new NotFoundObjectResult("Publisher not found!");
            }

            if (dto.Name != null) publisher.Name = dto.Name;
            if (dto.Email != null) publisher.Email = dto.Email;
            if (dto.Address != null) publisher.Address = dto.Address;
            if (dto.Contact != null) publisher.Contact = dto.Contact;

            await dbContext.SaveChangesAsync();
            return new OkObjectResult(publisher);
        }

        public async Task<IActionResult> DeletePublisher(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can delete publishers.");
            }

            var publisher = await dbContext.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return new NotFoundObjectResult("Publisher not found!");
            }

            dbContext.Publishers.Remove(publisher);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult("Deleted successfully");
        }
    }
}
