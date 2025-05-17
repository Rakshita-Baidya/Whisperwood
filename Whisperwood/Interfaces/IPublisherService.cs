using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IPublisherService
    {
        Task<IActionResult> AddPublisherAsync(Guid userId, PublisherDto dto);
        Task<IActionResult> GetAllPublishersAsync();
        Task<IActionResult> GetPublisherByIdAsync(Guid id);
        Task<IActionResult> UpdatePublisherAsync(Guid userId, Guid id, PublisherUpdateDto dto);
        Task<IActionResult> DeletePublisherAsync(Guid userId, Guid id);
    }
}
