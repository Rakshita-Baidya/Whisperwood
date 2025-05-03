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
    public class AnnouncementController : BaseController
    {
        private readonly WhisperwoodDbContext dbContext;

        public AnnouncementController(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddAnnouncement(AnnouncementDto dto)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return BadRequest("User not found. Are you sure you're logged in correctly?");
            }

            // Check if the user is an admin
            if (!user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can create announcements.");
            }

            if (dto.StartDate > dto.EndDate)
            {
                return BadRequest("StartDate cannot be after EndDate.");
            }
            var announcement = new Announcements
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Message = dto.Message,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                UserId = userId,
                Users = user
            };

            dbContext.Announcements.Add(announcement);
            await dbContext.SaveChangesAsync();
            return Ok(announcement);
        }


        [HttpGet("getall")]
        public async Task<IActionResult> GetAllAAnnouncements()
        {
            List<Announcements> announcementList = await dbContext.Announcements.Include(a => a.Users).ToListAsync();
            return Ok(announcementList);
        }

        [HttpGet("getbyid/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAnnouncementById(Guid id)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
            }
            Announcements? announcement = await dbContext.Announcements.Include(a => a.Users).FirstOrDefaultAsync(a => a.Id == id);
            return announcement != null ? Ok(announcement) : NotFound("Announcement not found!");
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAnnouncement(Guid id, AnnouncementUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
            }

            var announcement = await dbContext.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return NotFound("Announcement not found!");
            }
            if (dto.StartDate > dto.EndDate)
            {
                return BadRequest("StartDate cannot be after EndDate.");
            }

            if (dto.Title != null) announcement.Title = dto.Title;
            if (dto.Message != null) announcement.Message = dto.Message;
            if (dto.StartDate != null) announcement.StartDate = (DateOnly)dto.StartDate;
            if (dto.EndDate != null) announcement.EndDate = (DateOnly)dto.EndDate;

            await dbContext.SaveChangesAsync();
            return Ok(new
            {
                Id = announcement.Id,
                Title = announcement.Title,
                Message = announcement.Message,
                StartDate = announcement.StartDate,
                EndDate = announcement.EndDate,
                UserId = announcement.UserId
            });
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAnnouncement(Guid id)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can delete announcements.");
            }

            var announcement = await dbContext.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return NotFound("Announcement not found!");
            }

            dbContext.Announcements.Remove(announcement);
            await dbContext.SaveChangesAsync();
            return Ok("Deleted successfully");
        }
    }
}
