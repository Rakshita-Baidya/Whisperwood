using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistItemController : BaseController
    {
        private readonly IWishlistItemService wishlistService;

        public WishlistItemController(IWishlistItemService wishlistService)
        {
            this.wishlistService = wishlistService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist([FromBody] WishlistItemDTO dto)
        {
            return await wishlistService.AddToWishlistAsync(GetLoggedInUserId(), dto);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllWishlistItems()
        {
            return await wishlistService.GetAllWishlistItemsAsync(GetLoggedInUserId());
        }

        [HttpDelete("delete/{bookId}")]
        public async Task<IActionResult> DeleteWishlistItem(Guid bookId)
        {
            return await wishlistService.DeleteWishlistItemAsync(GetLoggedInUserId(), bookId);
        }

        [HttpGet("getbyuserid")]
        public async Task<IActionResult> GetByUserId()
        {
            var userId = GetLoggedInUserId();
            var wishlist = await wishlistService.GetByUserIdAsync(userId);
            if (wishlist == null)
                return NotFound("Wishlist not found.");
            return Ok(wishlist);
        }
    }
}
