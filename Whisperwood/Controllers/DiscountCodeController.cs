using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountCodeController : BaseController
    {
        private readonly IDiscountCodeService discountCodeService;

        public DiscountCodeController(IDiscountCodeService discountCodeService)
        {
            this.discountCodeService = discountCodeService;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddDiscountCode(DiscountCodeDto dto)
        {
            var userId = GetLoggedInUserId();
            return await discountCodeService.AddDiscountCode(userId, dto);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllDiscountCodes()
        {
            return await discountCodeService.GetAllDiscountCodes();
        }

        [HttpGet("getbyid/{id}")]
        [Authorize]
        public async Task<IActionResult> GetDiscountCodeById(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await discountCodeService.GetDiscountCodeById(userId, id);
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateDiscountCode(Guid id, DiscountCodeUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            return await discountCodeService.UpdateDiscountCode(userId, id, dto);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteDiscountCode(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await discountCodeService.DeleteDiscountCode(userId, id);
        }
    }
}
