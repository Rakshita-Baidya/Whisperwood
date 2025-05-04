using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Models;

namespace Whisperwood.Controllers
{
    public class OrderController : BaseController
    {
        private readonly WhisperwoodDbContext dbContext;

        public OrderController(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddOrder(OrderDto dto)
        {
            var userId = GetLoggedInUserId();

            var user = await dbContext.Users.FindAsync(userId);
            if (user == null) return NotFound("User not found.");

            var cart = await dbContext.Cart
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || cart.CartItems.Count == 0)
                return BadRequest("Your cart is empty.");

            decimal subTotal = 0;
            var orderItems = new List<OrderItem>();

            foreach (var cartItem in cart.CartItems)
            {
                var book = cartItem.Book;

                if (cartItem.Quantity > book.Stock)
                    return BadRequest($"Not enough stock for: {book.Title}");

                decimal unitPrice = book.Price;
                decimal itemSubtotal = unitPrice * cartItem.Quantity;
                subTotal += itemSubtotal;

                orderItems.Add(new OrderItem
                {
                    BookId = book.Id,
                    Quantity = cartItem.Quantity,
                    UnitPrice = unitPrice,
                    SubTotal = itemSubtotal
                });

                book.Stock -= cartItem.Quantity;
            }

            decimal discount = 0;

            int totalItems = cart.CartItems.Sum(i => i.Quantity);
            if (totalItems >= 5)
                discount += subTotal * 0.05m;

            int previousOrders = await dbContext.Orders.CountAsync(o => o.UserId == userId);
            if (previousOrders >= 10)
                discount += subTotal * 0.03m;

            decimal total = subTotal - discount;

            var order = new Orders
            {
                UserId = userId,
                SubTotal = subTotal,
                TotalAmount = total,
                Discount = discount,
                Status = Orders.OrderStatus.Pending,
                Date = DateOnly.FromDateTime(DateTime.Now),
                OrderedAt = DateTime.Now
            };

            await dbContext.Orders.AddAsync(order);
            await dbContext.SaveChangesAsync();

            foreach (var item in orderItems)
            {
                item.OrderId = order.Id;
            }

            await dbContext.OrderItem.AddRangeAsync(orderItems);
            dbContext.CartItem.RemoveRange(cart.CartItems);
            await dbContext.SaveChangesAsync();

            return Ok(new
            {
                OrderId = order.Id,
                Total = order.TotalAmount,
                Discount = order.Discount,
                Status = order.Status.ToString(),
                OrderedAt = order.OrderedAt.ToString("yyyy-MM-dd HH:mm:ss")
            });
        }
    }
}
