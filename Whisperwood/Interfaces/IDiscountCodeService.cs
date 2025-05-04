using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IDiscountCodeService
    {
        Task<IActionResult> AddDiscountCode(Guid userId, DiscountCodeDto dto);
        Task<IActionResult> GetAllDiscountCodes();
        Task<IActionResult> GetDiscountCodeById(Guid userId, Guid id);
        Task<IActionResult> UpdateDiscountCode(Guid userId, Guid id, DiscountCodeUpdateDto dto);
        Task<IActionResult> DeleteDiscountCode(Guid userId, Guid id);
    }
}
