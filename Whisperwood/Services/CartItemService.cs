using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;
using Whisperwood.Models;

namespace Whisperwood.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly WhisperwoodDbContext dbContext;

        public CartItemService(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> AddCartItemAsync(Guid userId, CartItemDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return new BadRequestObjectResult(new { message = "User not found. Are you sure you're logged in correctly?" });
            }

            var cart = await dbContext.Cart.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                return new BadRequestObjectResult(new { message = "Cart not found." });
            }

            var book = await dbContext.Books.FindAsync(dto.BookId);
            if (book == null)
            {
                return new BadRequestObjectResult(new { message = "Book not found." });
            }

            if (dto.Quantity <= 0)
            {
                return new BadRequestObjectResult(new { message = "Quantity must be greater than 0." });
            }

            var existingCartItem = await dbContext.CartItem.FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.BookId == dto.BookId);
            if (existingCartItem != null)
            {
                return new BadRequestObjectResult(new { message = "This book is already in your cart. You can update the quantity instead." });
            }

            if (dto.Quantity > book.Stock)
            {
                return new BadRequestObjectResult(new { message = "Quantity exceeds available stock." });
            }

            var subtotal = dto.Quantity * book.Price;
            var cartItem = new CartItem
            {
                BookId = dto.BookId,
                Quantity = dto.Quantity,
                CartId = cart.Id,
                SubTotal = subtotal,
            };

            dbContext.CartItem.Add(cartItem);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult(cartItem);
        }

        public async Task<IActionResult> GetAllCartItemsAsync(Guid userId)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return new BadRequestObjectResult(new { message = "User not found. Are you sure you're logged in correctly?" });
            }

            var cart = await dbContext.Cart.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                return new NotFoundObjectResult(new { message = "Cart not found." });
            }

            var cartItems = await dbContext.CartItem
                .Include(ci => ci.Book)
                .Where(ci => ci.CartId == cart.Id)
                .ToListAsync();
            return new OkObjectResult(cartItems);
        }

        public async Task<IActionResult> UpdateCartItemAsync(Guid userId, CartItemDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return new BadRequestObjectResult(new { message = "User not found." });
            }

            var cart = await dbContext.Cart.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                return new NotFoundObjectResult(new { message = "Cart not found." });
            }

            var cartItem = await dbContext.CartItem.FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.BookId == dto.BookId);
            if (cartItem == null)
            {
                return new NotFoundObjectResult(new { message = "Cart item not found." });
            }

            var book = await dbContext.Books.FindAsync(dto.BookId);
            if (book == null)
            {
                return new BadRequestObjectResult(new { message = "Book not found." });
            }

            if (dto.Quantity <= 0)
            {
                return new BadRequestObjectResult(new { message = "Quantity must be greater than 0." });
            }

            if (dto.Quantity > book.Stock)
            {
                return new BadRequestObjectResult(new { message = "Quantity exceeds stock." });
            }

            cartItem.Quantity = dto.Quantity;
            cartItem.SubTotal = dto.Quantity * book.Price;

            dbContext.CartItem.Update(cartItem);
            await dbContext.SaveChangesAsync();

            return new OkObjectResult(cartItem);
        }

        public async Task<IActionResult> DeleteCartItemAsync(Guid userId, Guid bookId)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return new BadRequestObjectResult(new { message = "User not found." });
            }

            var cart = await dbContext.Cart.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                return new NotFoundObjectResult(new { message = "Cart not found." });
            }

            var cartItem = await dbContext.CartItem.FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.BookId == bookId);
            if (cartItem == null)
            {
                return new NotFoundObjectResult(new { message = "Cart item not found." });
            }

            dbContext.CartItem.Remove(cartItem);
            await dbContext.SaveChangesAsync();

            return new OkObjectResult(new { message = "Deleted successfully." });
        }

        public async Task<Cart?> GetByUserIdAsync(Guid userId)
        {
            return await dbContext.Cart
                .Include(c => c.CartItems)
                .ThenInclude(i => i.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
    }
}
