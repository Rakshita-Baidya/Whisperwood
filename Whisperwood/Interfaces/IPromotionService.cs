using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IPromotionService
    {
        Task<IActionResult> AddPromotionAsync(Guid userId, PromotionDto dto);
        Task<IActionResult> GetAllPromotionsAsync();
        Task<IActionResult> GetPromotionByIdAsync(Guid userId, Guid id);
        Task<IActionResult> UpdatePromotionAsync(Guid userId, Guid id, PromotionUpdateDto dto);
        Task<IActionResult> DeletePromotionAsync(Guid userId, Guid id);
    }
}
