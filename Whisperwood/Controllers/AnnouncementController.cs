using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Models;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly WhisperwoodDbContext dbContext;

        public AnnouncementController(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddAnnouncement(AnnouncementDto dto)
        {
            var announcement = new Announcements
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Message = dto.Message,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
            };

            dbContext.Announcements.Add(announcement);
            await dbContext.SaveChangesAsync();
            return Ok(announcement);
        }


        [HttpGet("getall")]
        public async Task<IActionResult> GetAllAAnnouncements()
        {
            List<Announcements> announcementList = await dbContext.Announcements.ToListAsync();
            return Ok(announcementList);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetAnnouncementById(Guid id)
        {
            Announcements? announcement = await dbContext.Announcements.FirstOrDefaultAsync(a => a.Id == id);
            return announcement != null ? Ok(announcement) : NotFound("Announcement not found!");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateAnnouncement(Guid id, AnnouncementUpdateDto dto)
        {
            var announcement = await dbContext.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return NotFound("Announcement not found!");
            }

            if (dto.Title != null) announcement.Title = dto.Title;
            if (dto.Message != null) announcement.Message = dto.Message;
            if (dto.StartDate != null) announcement.StartDate = (DateOnly)dto.StartDate;
            if (dto.EndDate != null) announcement.EndDate = (DateOnly)dto.EndDate;

            await dbContext.SaveChangesAsync();
            return Ok(new
            {
                Id = announcement.Id,

                Announcement = new AnnouncementDto
                {
                    Title = announcement.Title!,
                    Message = announcement.Message,
                    StartDate = announcement.StartDate,
                    EndDate = announcement.EndDate
                }
            });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteAnnouncement(Guid id)
        {
            var announcement = await dbContext.Announcements.FindAsync(id);
            if (announcement == null)
            {
                return NotFound("Announcement not found!");
            }

            dbContext.Announcements.Remove(announcement);
            await dbContext.SaveChangesAsync();
            return Ok(new { Message = "Deleted successfully" });
        }
    }
}
