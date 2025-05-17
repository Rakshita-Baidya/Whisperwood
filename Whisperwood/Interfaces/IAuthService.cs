using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IAuthService
    {
        Task<IActionResult> RegisterUserAsync(UserDto dto);
        Task<IActionResult> LoginUserAsync(LoginDto loginDto);
        Task<IActionResult> GetUserAsync(Guid userId);
        Task<IActionResult> GetUserByIdAsync(Guid loggedInUserId, Guid userId);
        Task<IActionResult> GetAllUsersAsync(Guid userId);
        Task<IActionResult> UpdateUserAsync(Guid loggedInUserId, Guid userId, UserUpdateDto dto);
    }
}
