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
            return await cartItemService.AddCartItemAsync(userId, dto);
        }

        [HttpGet("getall")]
        [Authorize]
        public async Task<IActionResult> GetAllCartItems()
        {
            var userId = GetLoggedInUserId();
            return await cartItemService.GetAllCartItemsAsync(userId);
        }

        [HttpPut("update")]
        [Authorize]
        public async Task<IActionResult> UpdateCartItem(CartItemDto dto)
        {
            var userId = GetLoggedInUserId();
            return await cartItemService.UpdateCartItemAsync(userId, dto);
        }

        [HttpDelete("delete/{bookId}")]
        [Authorize]
        public async Task<IActionResult> DeleteCartItem(Guid bookId)
        {
            var userId = GetLoggedInUserId();
            return await cartItemService.DeleteCartItemAsync(userId, bookId);
        }


        [HttpGet("getbyuserid")]
        public async Task<IActionResult> GetByUserId()
        {
            var userId = GetLoggedInUserId();
            var cart = await cartItemService.GetByUserIdAsync(userId);
            if (cart == null)
                return NotFound("Cart not found.");
            return Ok(cart);
        }
    }
}
