using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : BaseController
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddOrderAsync(OrderDto dto)
        {
            var userId = GetLoggedInUserId();
            return await orderService.AddOrderAsync(userId, dto);
        }

        [HttpGet("getall")]
        [Authorize]

        public async Task<IActionResult> GetAllOrdersAsync()
        {
            var userId = GetLoggedInUserId();
            return await orderService.GetAllOrdersAsync(userId);
        }

        [HttpGet("getbyid/{id}")]
        [Authorize]

        public async Task<IActionResult> GetOrderByIdAsync(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await orderService.GetOrderByIdAsync(userId, id);
        }

        [HttpPut("update/{id}")]
        [Authorize]

        public async Task<IActionResult> UpdateOrderAsync(Guid id, OrderUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            return await orderService.UpdateOrderAsync(userId, id, dto);
        }

        [HttpGet("getbyuser/{userId}")]
        [Authorize]

        public async Task<IActionResult> GetOrdersByUserAsync(Guid userId)
        {
            var requestingUserId = GetLoggedInUserId();
            return await orderService.GetOrdersByUserAsync(requestingUserId, userId);
        }
    }
}
