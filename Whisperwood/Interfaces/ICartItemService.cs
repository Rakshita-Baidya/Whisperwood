using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface ICartItemService
    {
        Task<IActionResult> AddCartItem(Guid userId, CartItemDto dto);
        Task<IActionResult> GetAllCartItems(Guid userId);
        Task<IActionResult> UpdateCartItem(Guid userId, CartItemDto dto);
        Task<IActionResult> DeleteCartItem(Guid userId, Guid bookId);
    }
}
