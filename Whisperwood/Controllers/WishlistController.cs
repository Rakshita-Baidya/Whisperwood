using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Models;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WishlistItemController : BaseController
    {
        private readonly WhisperwoodDbContext dbContext;

        public WishlistItemController(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToWishlist(WishlistItemDTO dto)
        {
            var userId = GetLoggedInUserId();
            var customer = await dbContext.Users.FindAsync(userId);
            if (customer == null)
            {
                return BadRequest("Customer not found. Are you sure you're logged in?");
            }

            var wishlist = await dbContext.Wishlist.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                  

                };
                dbContext.Wishlist.Add(wishlist);
                await dbContext.SaveChangesAsync();
            }

            var book = await dbContext.Books.FindAsync(dto.BookId);
            if (book == null)
            {
                return BadRequest("Book not found.");
            }

            var existingItem = await dbContext.WishlistItem
                .FirstOrDefaultAsync(wi => wi.WishlistId == wishlist.Id && wi.BookId == dto.BookId);

            if (existingItem != null)
            {
                return BadRequest("This book is already in your wishlist.");
            }

            var wishlistItem = new WishlistItem
            {
                WishlistId = Guid.NewGuid(),
                Wishlist = wishlist,
                BookId = dto.BookId
            };

            dbContext.WishlistItem.Add(wishlistItem);
            await dbContext.SaveChangesAsync();

            return Ok(wishlistItem);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllWishlistItems()
        {
            var userId = GetLoggedInUserId();
            var wishlist = await dbContext.Wishlist
                .Include(w => w.WishListItems)
                .ThenInclude(i => i.Book)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
            {
                return Ok("Wishlist is empty.");
            }

            return Ok(wishlist.WishListItems);
        }

        [HttpDelete("delete/{bookId}")]
        public async Task<IActionResult> DeleteWishlistItem(Guid bookId)
        {
            var userId = GetLoggedInUserId();
            var wishlist = await dbContext.Wishlist.FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
                return NotFound("Wishlist not found.");

            var item = await dbContext.WishlistItem
                .FirstOrDefaultAsync(i => i.WishlistId == wishlist.Id && i.BookId == bookId);

            if (item == null)
                return NotFound("Item not found in wishlist.");

            dbContext.WishlistItem.Remove(item);
            await dbContext.SaveChangesAsync();

            return Ok("Wishlist item deleted successfully.");
        }
    }
}
