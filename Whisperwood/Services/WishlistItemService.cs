using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;
using Whisperwood.Models;

namespace Whisperwood.Services
{
    public class WishlistItemService : IWishlistItemService
    {
        private readonly WhisperwoodDbContext dbContext;

        public WishlistItemService(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> AddToWishlistAsync(Guid userId, WishlistItemDTO dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
                return new BadRequestObjectResult(new
                {
                    message = "User not found. Are you logged in?"
                });

            var wishlist = await dbContext.Wishlist
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
                return new BadRequestObjectResult(new
                {
                    message = "Wishlist not found. Please contact support."
                });

            var book = await dbContext.Books.FindAsync(dto.BookId);
            if (book == null)
                return new BadRequestObjectResult(new
                {
                    message = "Book not found."
                });

            var existingItem = await dbContext.WishlistItem
                .FirstOrDefaultAsync(wi => wi.WishlistId == wishlist.Id && wi.BookId == dto.BookId);

            if (existingItem != null)
                return new BadRequestObjectResult(new
                {
                    message = "This book is already in your wishlist."
                });

            var wishlistItem = new WishlistItem
            {
                WishlistId = wishlist.Id,
                BookId = dto.BookId
            };

            dbContext.WishlistItem.Add(wishlistItem);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult(wishlistItem);
        }

        public async Task<IActionResult> GetAllWishlistItemsAsync(Guid userId)
        {
            var wishlist = await dbContext.Wishlist
                .Include(w => w.WishListItems)
                .ThenInclude(wi => wi.Book)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null || !wishlist.WishListItems.Any())
                return new OkObjectResult(new List<WishlistItem>());

            return new OkObjectResult(wishlist.WishListItems);
        }

        public async Task<IActionResult> DeleteWishlistItemAsync(Guid userId, Guid bookId)
        {
            var wishlist = await dbContext.Wishlist
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
                return new NotFoundObjectResult(new
                {
                    message = "Wishlist not found."
                });

            var item = await dbContext.WishlistItem
                .FirstOrDefaultAsync(wi => wi.WishlistId == wishlist.Id && wi.BookId == bookId);

            if (item == null)
                return new NotFoundObjectResult(new
                {
                    message = "Item not found in wishlist."
                });

            dbContext.WishlistItem.Remove(item);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult(new
            {
                message = "Wishlist item deleted successfully."
            });
        }

        public async Task<Wishlist?> GetByUserIdAsync(Guid userId)
        {
            return await dbContext.Wishlist
                .Include(w => w.WishListItems)
                .ThenInclude(i => i.Book)
                .ThenInclude(c => c.CoverImage)
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }
    }
}
