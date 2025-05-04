using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IDiscountCodeService
    {
        Task<IActionResult> AddDiscountCodeAsync(Guid userId, DiscountCodeDto dto);
        Task<IActionResult> GetAllDiscountCodesAsync();
        Task<IActionResult> GetDiscountCodeByIdAsync(Guid userId, Guid id);
        Task<IActionResult> UpdateDiscountCodeAsync(Guid userId, Guid id, DiscountCodeUpdateDto dto);
        Task<IActionResult> DeleteDiscountCodeAsync(Guid userId, Guid id);
    }
}
