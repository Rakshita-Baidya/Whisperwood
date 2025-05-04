using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;
using Whisperwood.Models;

namespace Whisperwood.Services
{
    public class OrderService : IOrderService
    {
        private readonly WhisperwoodDbContext dbContext;
        private readonly IBillService billService;

        public OrderService(WhisperwoodDbContext dbContext, IBillService billService)
        {
            this.dbContext = dbContext;
            this.billService = billService;
        }

        public async Task<IActionResult> AddOrderAsync(Guid userId)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
                return new NotFoundObjectResult("User not found.");

            var cart = await dbContext.Cart
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                return new BadRequestObjectResult("Your cart is empty.");

            // Validate stock and calculate subtotal
            decimal subTotal = 0;
            var orderItems = new List<OrderItem>();

            foreach (var cartItem in cart.CartItems)
            {
                var book = cartItem.Book;
                if (cartItem.Quantity > book.Stock)
                    return new BadRequestObjectResult($"Not enough stock for: {book.Title}");

                decimal itemSubtotal = book.Price * cartItem.Quantity;
                subTotal += itemSubtotal;

                orderItems.Add(new OrderItem
                {
                    BookId = book.Id,
                    Quantity = cartItem.Quantity,
                    UnitPrice = book.Price,
                    SubTotal = itemSubtotal
                });

                book.Stock -= cartItem.Quantity;
            }

            // Calculate discounts
            decimal discount = 0;
            int totalItems = cart.CartItems.Sum(i => i.Quantity);
            if (totalItems >= 5)
                discount += subTotal * 0.05m;

            int previousOrders = await dbContext.Orders.CountAsync(o => o.UserId == userId);
            if (previousOrders >= 10)
                discount += subTotal * 0.1m;

            decimal total = subTotal - discount;

            // Create order
            var order = new Orders
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SubTotal = subTotal,
                TotalAmount = total,
                Discount = discount,
                Status = Orders.OrderStatus.Pending,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                OrderedAt = DateTime.UtcNow,
                OrderItems = orderItems
            };

            // Create bill
            var bill = new Bill
            {
                OrderId = order.Id,
                ClaimCode = Guid.NewGuid().ToString().Substring(0, 6),
                PickUpDate = order.Date.AddDays(2)
            };

            // Save order and bill
            dbContext.Orders.Add(order);
            dbContext.Bill.Add(bill);
            dbContext.CartItem.RemoveRange(cart.CartItems);
            await dbContext.SaveChangesAsync();

            //Generate bill
            var pdfBytes = await billService.GenerateBillPdfAsync(order.Id);

            // Send email with bill
            await billService.SendBillEmailAsync(order.Id, user.Email, pdfBytes);

            return new OkObjectResult(new
            {
                OrderId = order.Id,
                Total = order.TotalAmount,
                Discount = order.Discount,
                Status = order.Status.ToString(),
                OrderedAt = order.OrderedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                ClaimCode = bill.ClaimCode,
                PickUpDate = bill.PickUpDate.ToString("yyyy-MM-dd")
            });

        }

        public async Task<IActionResult> GetAllOrdersAsync(Guid userId)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
                return new NotFoundObjectResult("User not found.");

            var orders = await dbContext.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .ToListAsync();

            return new OkObjectResult(orders);
        }

        public async Task<IActionResult> GetOrderByIdAsync(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
                return new NotFoundObjectResult("User not found.");

            var order = await dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            return order != null ? new OkObjectResult(order) : new NotFoundObjectResult("Order not found!");
        }

        public async Task<IActionResult> UpdateOrderAsync(Guid userId, Guid id, OrderUpdateDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
                return new NotFoundObjectResult("User not found.");

            var order = await dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            var bill = await dbContext.Bill.FirstOrDefaultAsync(b => b.OrderId == order.Id);

            if (order == null)
                return new NotFoundObjectResult("Order not found!");

            if (dto.Status.HasValue)
            {
                if (order.Status == Orders.OrderStatus.Cancelled || order.Status == Orders.OrderStatus.Fulfilled)
                    return new BadRequestObjectResult("Cannot update a cancelled or fulfilled order.");

                if (dto.Status.Value == Orders.OrderStatus.Fulfilled)
                {
                    if (string.IsNullOrEmpty(dto.ClaimCode))
                        return new BadRequestObjectResult("Claim code is required to fulfill the order.");

                    if (bill == null || bill.ClaimCode != dto.ClaimCode)
                        return new BadRequestObjectResult("Invalid claim code.");
                }

                if (dto.Status.Value == Orders.OrderStatus.Cancelled)
                {
                    foreach (var item in order.OrderItems)
                    {
                        item.Book.Stock += item.Quantity;
                    }
                }

                order.Status = dto.Status.Value;
            }

            await dbContext.SaveChangesAsync();
            return new OkObjectResult(new
            {
                OrderId = order.Id,
                Status = order.Status.ToString(),
                Total = order.TotalAmount,
                Discount = order.Discount,
                OrderedAt = order.OrderedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                ClaimCode = bill?.ClaimCode,
                PickUpDate = bill?.PickUpDate.ToString("yyyy-MM-dd"),
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    BookId = oi.Book.Id,
                    Quantity = oi.Quantity,
                }).ToList()
            });
        }

        public async Task<IActionResult> GetOrdersByUserAsync(Guid requestingUserId, Guid targetUserId)
        {
            var requestingUser = await dbContext.Users.FindAsync(requestingUserId);
            if (requestingUser == null || !requestingUser.IsAdmin.GetValueOrDefault(false))
                return new UnauthorizedObjectResult("Only admins can view orders for other users.");

            var targetUser = await dbContext.Users.FindAsync(targetUserId);
            if (targetUser == null)
                return new NotFoundObjectResult("Target user not found.");

            var orders = await dbContext.Orders
                .Where(o => o.UserId == targetUserId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .ToListAsync();

            return new OkObjectResult(orders);
        }
    }
}