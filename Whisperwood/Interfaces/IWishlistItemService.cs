using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Models;

namespace Whisperwood.Interfaces
{
    public interface IWishlistItemService
    {
        Task<IActionResult> AddToWishlistAsync(Guid userId, WishlistItemDTO dto);
        Task<IActionResult> GetAllWishlistItemsAsync(Guid userId);
        Task<IActionResult> DeleteWishlistItemAsync(Guid userId, Guid bookId);
        Task<Wishlist?> GetByUserIdAsync(Guid userId);
    }
}
