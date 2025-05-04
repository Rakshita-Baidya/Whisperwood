using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnnouncementController : BaseController
    {
        private readonly IAnnouncementService announcementService;

        public AnnouncementController(IAnnouncementService announcementService)
        {
            this.announcementService = announcementService;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddAnnouncement(AnnouncementDto dto)
        {
            var userId = GetLoggedInUserId();
            return await announcementService.AddAnnouncementAsync(userId, dto);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllAnnouncements()
        {
            return await announcementService.GetAllAnnouncementsAsync();
        }

        [HttpGet("getbyid/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAnnouncementById(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await announcementService.GetAnnouncementByIdAsync(userId, id);
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAnnouncement(Guid id, AnnouncementUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            return await announcementService.UpdateAnnouncementAsync(userId, id, dto);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAnnouncement(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await announcementService.DeleteAnnouncementAsync(userId, id);
        }
    }
}
