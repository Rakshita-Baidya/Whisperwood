using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IAnnouncementService
    {
        Task<IActionResult> AddAnnouncementAsync(Guid userId, AnnouncementDto dto);
        Task<IActionResult> GetAllAnnouncementsAsync();
        Task<IActionResult> GetAnnouncementByIdAsync(Guid userId, Guid id);
        Task<IActionResult> UpdateAnnouncementAsync(Guid userId, Guid id, AnnouncementUpdateDto dto);
        Task<IActionResult> DeleteAnnouncementAsync(Guid userId, Guid id);
    }
}
