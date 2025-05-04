using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IAnnouncementService
    {
        Task<IActionResult> AddAnnouncement(Guid userId, AnnouncementDto dto);
        Task<IActionResult> GetAllAnnouncements();
        Task<IActionResult> GetAnnouncementById(Guid userId, Guid id);
        Task<IActionResult> UpdateAnnouncement(Guid userId, Guid id, AnnouncementUpdateDto dto);
        Task<IActionResult> DeleteAnnouncement(Guid userId, Guid id);
    }
}
