using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IPromotionService
    {
        Task<IActionResult> AddPromotion(Guid userId, PromotionDto dto);
        Task<IActionResult> GetAllPromotions();
        Task<IActionResult> GetPromotionById(Guid userId, Guid id);
        Task<IActionResult> UpdatePromotion(Guid userId, Guid id, PromotionUpdateDto dto);
        Task<IActionResult> DeletePromotion(Guid userId, Guid id);
    }
}
