using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Models;

namespace Whisperwood.Interfaces
{
    public interface IPromotionService
    {
        Task<IActionResult> AddPromotionAsync(Guid userId, PromotionDto dto);
        Task<IActionResult> GetAllPromotionsAsync();
        Task<IActionResult> GetPromotionByIdAsync(Guid id);
        Task<IActionResult> UpdatePromotionAsync(Guid userId, Guid id, PromotionUpdateDto dto);
        Task<IActionResult> DeletePromotionAsync(Guid userId, Guid id);
        Task<(Promotions? Promotion, string? Error)> ValidatePromoCodeAsync(Guid userId, string? promoCode);
    }
}
