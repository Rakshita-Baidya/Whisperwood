//using Microsoft.EntityFrameworkCore;
//using Whisperwood.DatabaseContext;
//using Whisperwood.DTOs;
//using Whisperwood.Interfaces;
//using Whisperwood.Models;

//namespace Whisperwood.Services
//{
//    public class OrderService : IOrderService
//    {
//        private readonly WhisperwoodDbContext dbContext;

//        public OrderService(WhisperwoodDbContext dbContext)
//        {
//            this.dbContext = dbContext;
//        }

//        public async Task<OrderDto> AddOrderAsync(Guid userId)
//        {
//            var user = await dbContext.Users.FindAsync(userId);
//            if (user == null) throw new Exception("User not found.");

//            var cart = await dbContext.Cart
//                .Include(c => c.CartItems)
//                .ThenInclude(ci => ci.Book)
//                .FirstOrDefaultAsync(c => c.UserId == userId);

//            if (cart == null || cart.CartItems.Count == 0)
//                throw new Exception("Your cart is empty.");

//            // Calculate subtotal and build order items
//            decimal subTotal = 0;
//            var orderItems = new List<OrderItem>();

//            foreach (var cartItem in cart.CartItems)
//            {
//                var book = cartItem.Book;

//                if (cartItem.Quantity > book.Stock)
//                    throw new Exception($"Not enough stock for: {book.Title}");

//                decimal itemSubtotal = book.Price * cartItem.Quantity;
//                subTotal += itemSubtotal;

//                orderItems.Add(new OrderItem
//                {
//                    BookId = book.Id,
//                    Quantity = cartItem.Quantity,
//                    SubTotal = itemSubtotal,
//                    UnitPrice = book.Price
//                });

//                book.Stock -= cartItem.Quantity;
//            }

//            // Calculate discounts
//            decimal discount = 0;
//            int totalItems = cart.CartItems.Sum(i => i.Quantity);
//            if (totalItems >= 5)
//                discount += subTotal * 0.05m;

//            int previousOrders = await dbContext.Orders.CountAsync(o => o.UserId == userId);
//            if (previousOrders >= 10)
//                discount += subTotal * 0.03m;

//            decimal total = subTotal - discount;

//            // Create order
//            var order = new Orders
//            {
//                UserId = userId,
//                SubTotal = subTotal,
//                Discount = discount,
//                TotalAmount = total,
//                Status = Orders.OrderStatus.Pending,
//                Date = DateOnly.FromDateTime(DateTime.Now),
//                OrderedAt = DateTime.Now
//            };

//            await dbContext.Orders.AddAsync(order);
//            await dbContext.SaveChangesAsync();

//            // Assign order ID to order items
//            foreach (var item in orderItems)
//            {
//                item.OrderId = order.Id;
//            }

//            await dbContext.OrderItem.AddRangeAsync(orderItems);
//            dbContext.CartItem.RemoveRange(cart.CartItems);
//            await dbContext.SaveChangesAsync();

//            return new Orders
//            {
//                Id = order.Id,
//                TotalAmount = order.TotalAmount,
//                Discount = discount,
//                Status = order.Status,
//                OrderedAt = order.OrderedAt
//            };
//        }
//    }
//}
