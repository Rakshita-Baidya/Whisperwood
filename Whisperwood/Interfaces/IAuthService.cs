using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;

namespace Whisperwood.Interfaces
{
    public interface IAuthService
    {
        Task<IActionResult> RegisterUser(UserDto dto);
        Task<IActionResult> LoginUser(LoginDto loginDto);
    }
}
