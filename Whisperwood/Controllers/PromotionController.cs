using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : BaseController
    {
        private readonly IPromotionService promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            this.promotionService = promotionService;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddPromotion(PromotionDto dto)
        {
            var userId = GetLoggedInUserId();
            return await promotionService.AddPromotionAsync(userId, dto);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllPromotions()
        {
            return await promotionService.GetAllPromotionsAsync();
        }

        [HttpGet("getbyid/{id}")]
        [Authorize]
        public async Task<IActionResult> GetPromotionById(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await promotionService.GetPromotionByIdAsync(userId, id);
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdatePromotion(Guid id, PromotionUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            return await promotionService.UpdatePromotionAsync(userId, id, dto);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeletePromotion(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await promotionService.DeletePromotionAsync(userId, id);
        }
    }
}
