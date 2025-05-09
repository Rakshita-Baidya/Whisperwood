using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Models;

namespace Whisperwood.Interfaces
{
    public interface ICartItemService
    {
        Task<IActionResult> AddCartItemAsync(Guid userId, CartItemDto dto);
        Task<IActionResult> GetAllCartItemsAsync(Guid userId);
        Task<IActionResult> UpdateCartItemAsync(Guid userId, CartItemUpdateDto dto);
        Task<IActionResult> DeleteCartItemAsync(Guid userId, Guid bookId);
        Task<Cart?> GetByUserIdAsync(Guid userId);
    }
}
