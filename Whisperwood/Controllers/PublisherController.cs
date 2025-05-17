using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublisherController : BaseController
    {
        private readonly IPublisherService publisherService;

        public PublisherController(IPublisherService publisherService)
        {
            this.publisherService = publisherService;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddPublisher(PublisherDto dto)
        {
            var userId = GetLoggedInUserId();
            return await publisherService.AddPublisherAsync(userId, dto);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllPublishers()
        {
            return await publisherService.GetAllPublishersAsync();
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetPublisherById(Guid id)
        {
            return await publisherService.GetPublisherByIdAsync(id);
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePublisher(Guid id, PublisherUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            return await publisherService.UpdatePublisherAsync(userId, id, dto);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePublisher(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await publisherService.DeletePublisherAsync(userId, id);
        }
    }
}
