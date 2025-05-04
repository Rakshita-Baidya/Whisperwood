using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Models;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartItemController : BaseController
    {
        private readonly WhisperwoodDbContext dbContext;

        public CartItemController(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }


        [HttpPost("add")]
        public async Task<IActionResult> AddCartItem(CartItemDto dto)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return BadRequest("User not found. Are you sure you're logged in correctly?");
            }
            var cart = await dbContext.Cart.FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null)
            {
                return BadRequest("Cart not found.");
            }

            Books book = await dbContext.Books.FindAsync(dto.BookId);

            if (dto.Quantity <= 0)
            {
                return BadRequest("Quantity must be greater than 0.");

            }

            var existingCartItem = await dbContext.CartItem.FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.BookId == dto.BookId);

            if (existingCartItem != null)
                return BadRequest("This book is already in your cart. You can update the quantity instead.");


            else if (dto.Quantity > book.Stock)
            {
                return BadRequest("Quantity exceeds available stock.");
            }

            var subtotal = dto.Quantity * book.Price;
            var cartItem = new CartItem
            {
                BookId = dto.BookId,
                Quantity = dto.Quantity,
                CartId = cart!.Id,
                SubTotal = subtotal,
            };

            dbContext.CartItem.Add(cartItem);
            await dbContext.SaveChangesAsync();
            return Ok(cartItem);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllCartItems()
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return BadRequest("User not found. Are you sure you're logged in correctly?");
            }
            var cart = await dbContext.Cart.FirstOrDefaultAsync(c => c.UserId == userId);

            var cartItems = await dbContext.CartItem
                .Include(ci => ci.Book)
                .Where(ci => ci.CartId == cart!.Id)
                .ToListAsync();
            return Ok(cartItems);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateCartItem(CartItemDto dto)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null) return BadRequest("User not found.");

            var cart = await dbContext.Cart.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null) return NotFound("Cart not found.");

            var cartItem = await dbContext.CartItem.FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.BookId == dto.BookId);
            if (cartItem == null) return NotFound("Cart item not found.");

            var book = await dbContext.Books.FindAsync(dto.BookId);
            if (book == null) return BadRequest("Book not found.");

            if (dto.Quantity <= 0) return BadRequest("Quantity must be greater than 0.");
            if (dto.Quantity > book.Stock) return BadRequest("Quantity exceeds stock.");

            cartItem.Quantity = dto.Quantity;
            cartItem.SubTotal = dto.Quantity * book.Price;

            dbContext.CartItem.Update(cartItem);
            await dbContext.SaveChangesAsync();

            return Ok(cartItem);
        }

        [HttpDelete("delete/{bookId}")]
        public async Task<IActionResult> DeleteCartItem(Guid bookId)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null) return BadRequest("User not found.");

            var cart = await dbContext.Cart.FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null) return NotFound("Cart not found.");

            var cartItem = await dbContext.CartItem.FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.BookId == bookId);
            if (cartItem == null) return NotFound("Cart item not found.");

            dbContext.CartItem.Remove(cartItem);
            await dbContext.SaveChangesAsync();

            return Ok("Deleted successfully.");
        }


    }
}
