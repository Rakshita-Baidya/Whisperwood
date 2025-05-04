using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IOrderService
    {
        Task<IActionResult> AddOrderAsync(Guid userId);
        Task<IActionResult> GetAllOrdersAsync(Guid userId);
        Task<IActionResult> GetOrderByIdAsync(Guid userId, Guid id);
        Task<IActionResult> UpdateOrderAsync(Guid userId, Guid id, OrderUpdateDto dto);
        Task<IActionResult> GetOrdersByUserAsync(Guid requestingUserId, Guid targetUserId);
    }
}
