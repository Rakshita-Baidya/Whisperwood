using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;
using Whisperwood.Models;
using Whisperwood.Signals;

namespace Whisperwood.Services
{
    public class OrderService : IOrderService
    {
        private readonly WhisperwoodDbContext dbContext;
        private readonly IBillService billService;
        private readonly IPromotionService promotionService;
        private readonly IDiscountCodeService discountService;
        private readonly IHubContext<OrderHub> hubContext;

        public OrderService(WhisperwoodDbContext dbContext, IBillService billService, IPromotionService promotionService, IDiscountCodeService discountService, IHubContext<OrderHub> hubContext)
        {
            this.dbContext = dbContext;
            this.billService = billService;
            this.promotionService = promotionService;
            this.discountService = discountService;
            this.hubContext = hubContext;
        }

        private decimal CalculateEffectivePrice(Books book)
        {
            if (book.IsOnSale && book.DiscountPercentage > 0)
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                bool isSaleActive = (!book.DiscountStartDate.HasValue || book.DiscountStartDate <= today) &&
                                   (!book.DiscountEndDate.HasValue || book.DiscountEndDate >= today);
                if (isSaleActive)
                {
                    return book.Price * (1 - book.DiscountPercentage / 100m);
                }
            }
            return book.Price;
        }

        public async Task<IActionResult> AddOrderAsync(Guid userId, OrderDto dto)
        {
            Console.WriteLine($"OrderService: Processing order for user {userId}");

            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
                return new NotFoundObjectResult(new { message = "User not found." });

            var cart = await dbContext.Cart
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
                return new BadRequestObjectResult(new { message = "Your cart is empty." });

            // Validate promo code
            var (appliedPromotion, promoError) = await promotionService.ValidatePromoCodeAsync(userId, dto.PromoCode);
            if (promoError != null)
                return new BadRequestObjectResult(promoError);

            // Validate stock and calculate subtotal
            var orderedBooks = new List<string>();
            decimal subTotal = 0;
            decimal bookDiscount = 0;
            var orderItems = new List<OrderItem>();

            foreach (var cartItem in cart.CartItems)
            {
                var book = cartItem.Book;
                if (cartItem.Quantity > book.Stock)
                    return new BadRequestObjectResult($"Not enough stock for: {book.Title}");

                orderedBooks.Add(book.Title);

                decimal effectivePrice = CalculateEffectivePrice(book);
                decimal originalPrice = book.Price;
                decimal itemBookDiscount = (originalPrice - effectivePrice) * cartItem.Quantity;
                decimal itemSubtotal = effectivePrice * cartItem.Quantity;

                subTotal += itemSubtotal;
                bookDiscount += itemBookDiscount;

                orderItems.Add(new OrderItem
                {
                    BookId = book.Id,
                    Quantity = cartItem.Quantity,
                    UnitPrice = effectivePrice,
                    SubTotal = itemSubtotal
                });

                book.Stock -= cartItem.Quantity;
                book.SalesCount += cartItem.Quantity;
                if (book.Stock == 0) book.AvailabilityStatus = false;
            }

            // Calculate additional discounts
            int totalItems = cart.CartItems.Sum(i => i.Quantity);
            decimal totalDiscount = discountService.CalculateDiscount(subTotal, totalItems, user.OrdersCount, appliedPromotion);
            decimal total = subTotal - (totalDiscount - bookDiscount);

            // Calculate stacked discounts for bill
            decimal currentSubTotal = subTotal;
            decimal promoDiscount = discountService.GetPromotionDiscount(currentSubTotal, appliedPromotion);
            currentSubTotal -= promoDiscount;
            decimal bulkDiscount = discountService.GetBulkDiscount(currentSubTotal, totalItems);
            currentSubTotal -= bulkDiscount;
            decimal loyalDiscount = discountService.GetLoyalDiscount(currentSubTotal, user.OrdersCount);

            // Create order
            var order = new Orders
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                SubTotal = subTotal,
                TotalAmount = total,
                Discount = totalDiscount,
                Status = Orders.OrderStatus.Pending,
                Date = DateOnly.FromDateTime(DateTime.UtcNow),
                OrderedAt = DateTime.UtcNow,
                OrderItems = orderItems,
                PromoCode = dto.PromoCode
            };

            // Create bill
            var bill = new Bill
            {
                OrderId = order.Id,
                ClaimCode = Guid.NewGuid().ToString().Substring(0, 6),
                PickUpDate = order.Date.AddDays(2),
                PromoCode = dto.PromoCode,
                PromoDiscount = promoDiscount,
                BulkDiscount = bulkDiscount,
                LoyalDiscount = loyalDiscount,
                BookDiscount = bookDiscount
            };

            // Save order and bill
            dbContext.Orders.Add(order);
            dbContext.Bill.Add(bill);
            dbContext.CartItem.RemoveRange(cart.CartItems);
            user.OrdersCount += 1;
            await dbContext.SaveChangesAsync();

            // Broadcast order announcement via notifier
            string message = $"New order placed with books: {string.Join(", ", orderedBooks)}";
            await hubContext.Clients.All.SendAsync("ReceiveOrderUpdate", message);

            Console.WriteLine(message);
            // Generate bill
            var pdfBytes = await billService.GenerateBillPdfAsync(order.Id);

            // Send email with bill
            try
            {
                await billService.SendBillEmailAsync(order.Id, user.Email, pdfBytes);
            }
            catch (InvalidOperationException ex)
            {
                return new OkObjectResult(new
                {
                    EmailError = $"Failed to send bill email: {ex.Message}",
                    OrderId = order.Id,
                    Total = order.TotalAmount,
                    PromoCode = bill.PromoCode,
                    Discount = order.Discount,
                    BookDiscount = bill.BookDiscount,
                    PromoDiscount = bill.PromoDiscount,
                    BulkDiscount = bill.BulkDiscount,
                    LoyalDiscount = bill.LoyalDiscount,
                    Status = order.Status.ToString(),
                    OrderedAt = order.OrderedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    ClaimCode = bill.ClaimCode,
                    PickUpDate = bill.PickUpDate.ToString("yyyy-MM-dd"),
                    OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                    {
                        BookId = oi.Book.Id,
                        Quantity = oi.Quantity,
                    }).ToList()
                });
            }

            return new OkObjectResult(new
            {
                OrderId = order.Id,
                Total = order.TotalAmount,
                PromoCode = bill.PromoCode,
                Discount = order.Discount,
                BookDiscount = bill.BookDiscount,
                PromoDiscount = bill.PromoDiscount,
                BulkDiscount = bill.BulkDiscount,
                LoyalDiscount = bill.LoyalDiscount,
                Status = order.Status.ToString(),
                OrderedAt = order.OrderedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                ClaimCode = bill.ClaimCode,
                PickUpDate = bill.PickUpDate.ToString("yyyy-MM-dd"),
                OrderItems = order.OrderItems.Select(oi => new OrderItemDto
                {
                    BookId = oi.Book.Id,
                    Quantity = oi.Quantity,
                }).ToList()
            });
        }

        public async Task<IActionResult> GetAllOrdersAsync(Guid userId)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
                return new NotFoundObjectResult(new
                {
                    message = "User not found."
                });

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
                return new NotFoundObjectResult(new
                {
                    message = "User not found."
                });

            var order = await dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            return order != null ? new OkObjectResult(order) : new NotFoundObjectResult(new
            {
                message = "Order not found!"
            });
        }

        public async Task<IActionResult> UpdateOrderAsync(Guid userId, Guid id, OrderUpdateDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
                return new NotFoundObjectResult(new
                {
                    message = "User not found."
                });

            var order = await dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            var bill = await dbContext.Bill.FirstOrDefaultAsync(b => b.OrderId == order.Id);

            if (order == null)
                return new NotFoundObjectResult(new
                {
                    message = "Order not found!"
                });

            if (dto.Status.HasValue)
            {
                if (order.Status == Orders.OrderStatus.Cancelled || order.Status == Orders.OrderStatus.Fulfilled)
                    return new BadRequestObjectResult(new
                    {
                        message = "Cannot update a cancelled or fulfilled order."
                    });

                if (dto.Status.Value == Orders.OrderStatus.Fulfilled)
                {
                    if (string.IsNullOrEmpty(dto.ClaimCode))
                        return new BadRequestObjectResult(new
                        {
                            message = "Claim code is required to fulfill the order."
                        });

                    if (bill == null || bill.ClaimCode != dto.ClaimCode)
                        return new BadRequestObjectResult(new
                        {
                            message = "Invalid claim code."
                        });
                    user.OrdersCount += 1;
                }

                if (dto.Status.Value == Orders.OrderStatus.Cancelled)
                {
                    foreach (var item in order.OrderItems)
                    {
                        item.Book.Stock += item.Quantity;
                        item.Book.SalesCount -= item.Quantity;
                        if (item.Book.Stock > 0) item.Book.AvailabilityStatus = true;
                    }
                    user.OrdersCount -= 1;
                }

                order.Status = dto.Status.Value;
            }

            await dbContext.SaveChangesAsync();
            return new OkObjectResult(new
            {
                OrderId = order.Id,
                Status = order.Status.ToString(),
                PromoCode = order.PromoCode,
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
            if (requestingUser != null)
            {
                if (!requestingUser.IsAdmin.GetValueOrDefault(false) && !requestingUser.IsStaff.GetValueOrDefault(false) && requestingUserId != targetUserId)
                {
                    return new UnauthorizedObjectResult(new
                    {
                        message = "Only admins, staff or own user can view user orders."
                    });
                }
            }

            var targetUser = await dbContext.Users.FindAsync(targetUserId);
            if (targetUser == null)
                return new NotFoundObjectResult(new
                {
                    message = "Target user not found."
                });

            var orders = await dbContext.Orders
                .Where(o => o.UserId == targetUserId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .ToListAsync();

            return new OkObjectResult(orders);
        }
    }
}