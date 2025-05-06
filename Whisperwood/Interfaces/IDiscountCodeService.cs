using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Models;

namespace Whisperwood.Interfaces
{
    public interface IDiscountCodeService
    {
        decimal CalculateDiscount(decimal subTotal, int totalItems, int userOrdersCount, Promotions? promotion);
        decimal GetPromotionDiscount(decimal subTotal, Promotions? promotion);
        Task<IActionResult> AddDiscountCodeAsync(Guid userId, DiscountCodeDto dto);
        Task<IActionResult> GetAllDiscountCodesAsync();
        Task<IActionResult> GetDiscountCodeByIdAsync(Guid userId, Guid id);
        Task<IActionResult> UpdateDiscountCodeAsync(Guid userId, Guid id, DiscountCodeUpdateDto dto);
        Task<IActionResult> DeleteDiscountCodeAsync(Guid userId, Guid id);
    }
}
