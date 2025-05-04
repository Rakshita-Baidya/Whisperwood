using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;
using Whisperwood.Models;

namespace Whisperwood.Services
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly WhisperwoodDbContext dbContext;

        public AnnouncementService(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> AddAnnouncementAsync(Guid userId, AnnouncementDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return new BadRequestObjectResult("User not found. Are you sure you're logged in correctly?");
            }

            if (!user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can create announcements.");
            }

            if (dto.StartDate > dto.EndDate)
            {
                return new BadRequestObjectResult("StartDate cannot be after EndDate.");
            }

            var announcement = new Announcements
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Message = dto.Message,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                UserId = userId,
                User = user
            };

            dbContext.Announcements.Add(announcement);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult(announcement);
        }

        public async Task<IActionResult> GetAllAnnouncementsAsync()
        {
            var announcementList = await dbContext.Announcements.Include(a => a.User).ToListAsync();
            return new OkObjectResult(announcementList);
        }

        public async Task<IActionResult> GetAnnouncementByIdAsync(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can view announcements.");
            }

            var announcement = await dbContext.Announcements.Include(a => a.User).FirstOrDefaultAsync(a => a.Id == id);
            return announcement != null ? new OkObjectResult(announcement) : new NotFoundObjectResult("Announcement not found!");
        }

        public async Task<IActionResult> UpdateAnnouncementAsync(Guid userId, Guid id, AnnouncementUpdateDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can update announcements.");
            }

            var announcement = await dbContext.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return new NotFoundObjectResult("Announcement not found!");
            }

            if (dto.StartDate > dto.EndDate)
            {
                return new BadRequestObjectResult("StartDate cannot be after EndDate.");
            }

            if (dto.Title != null) announcement.Title = dto.Title;
            if (dto.Message != null) announcement.Message = dto.Message;
            if (dto.StartDate != null) announcement.StartDate = (DateOnly)dto.StartDate;
            if (dto.EndDate != null) announcement.EndDate = (DateOnly)dto.EndDate;

            await dbContext.SaveChangesAsync();
            return new OkObjectResult(new
            {
                Id = announcement.Id,
                Title = announcement.Title,
                Message = announcement.Message,
                StartDate = announcement.StartDate,
                EndDate = announcement.EndDate,
                UserId = announcement.UserId
            });
        }

        public async Task<IActionResult> DeleteAnnouncementAsync(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can delete announcements.");
            }

            var announcement = await dbContext.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return new NotFoundObjectResult("Announcement not found!");
            }

            dbContext.Announcements.Remove(announcement);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult("Deleted successfully");
        }
    }
}
