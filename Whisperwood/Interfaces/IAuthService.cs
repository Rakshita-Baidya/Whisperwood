using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IAuthService
    {
        Task<IActionResult> RegisterUserAsync(UserDto dto);
        Task<IActionResult> LoginUserAsync(LoginDto loginDto);
        Task<IActionResult> GetUserByIdAsync(Guid userId);
    }
}
