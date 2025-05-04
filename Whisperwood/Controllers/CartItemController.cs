using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : BaseController
    {
        private readonly ICartItemService cartItemService;

        public CartItemController(ICartItemService cartItemService)
        {
            this.cartItemService = cartItemService;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddCartItem(CartItemDto dto)
        {
            var userId = GetLoggedInUserId();
            return await cartItemService.AddCartItem(userId, dto);
        }

        [HttpGet("getall")]
        [Authorize]
        public async Task<IActionResult> GetAllCartItems()
        {
            var userId = GetLoggedInUserId();
            return await cartItemService.GetAllCartItems(userId);
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateCartItem(CartItemDto dto)
        {
            var userId = GetLoggedInUserId();
            return await cartItemService.UpdateCartItem(userId, dto);
        }

        [HttpDelete("delete/{bookId}")]
        [Authorize]
        public async Task<IActionResult> DeleteCartItem(Guid bookId)
        {
            var userId = GetLoggedInUserId();
            return await cartItemService.DeleteCartItem(userId, bookId);
        }
    }
}
