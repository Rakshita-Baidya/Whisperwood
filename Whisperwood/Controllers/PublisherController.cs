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
            return await publisherService.AddPublisher(userId, dto);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllPublishers()
        {
            return await publisherService.GetAllPublishers();
        }

        [HttpGet("getbyid/{id}")]
        [Authorize]
        public async Task<IActionResult> GetPublisherById(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await publisherService.GetPublisherById(userId, id);
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePublisher(Guid id, PublisherUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            return await publisherService.UpdatePublisher(userId, id, dto);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePublisher(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await publisherService.DeletePublisher(userId, id);
        }
    }
}
