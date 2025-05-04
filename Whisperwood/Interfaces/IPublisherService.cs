using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IPublisherService
    {
        Task<IActionResult> AddPublisher(Guid userId, PublisherDto dto);
        Task<IActionResult> GetAllPublishers();
        Task<IActionResult> GetPublisherById(Guid userId, Guid id);
        Task<IActionResult> UpdatePublisher(Guid userId, Guid id, PublisherUpdateDto dto);
        Task<IActionResult> DeletePublisher(Guid userId, Guid id);
    }
}
