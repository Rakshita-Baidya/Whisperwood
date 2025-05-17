using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(UserDto dto)
        {
            return await authService.RegisterUserAsync(dto);
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginDto loginDto)
        {
            var result = await authService.LoginUserAsync(loginDto);

            if (result is OkObjectResult okResult)
            {
                var response = okResult.Value as dynamic;
                string token = response.Token;

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(60)
                };
                Response.Cookies.Append("JwtToken", token, cookieOptions);

                return Ok(new { message = "Login successful", token });
            }
            else if (result is UnauthorizedObjectResult unauthorizedResult)
            {
                return Unauthorized(new { message = unauthorizedResult.Value?.ToString() });
            }

            return BadRequest(new { message = "An error occurred during login." });
        }

        [HttpGet("user")]
        [Authorize]

        public async Task<IActionResult> GetUser()
        {
            var userId = GetLoggedInUserId();
            return await authService.GetUserAsync(userId);
        }

        [HttpGet("getallusers")]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var userId = GetLoggedInUserId();
            return await authService.GetAllUsersAsync(userId);
        }

        [HttpGet("getuserbyid/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var loggedInUserId = GetLoggedInUserId();
            return await authService.GetUserByIdAsync(loggedInUserId, userId);
        }

        [HttpPut("updateuserbyid/{userId}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid userId, UserUpdateDto dto)
        {
            var loggedInUserId = GetLoggedInUserId();
            return await authService.UpdateUserAsync(loggedInUserId, userId, dto);
        }
    }
}
