using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;
using Whisperwood.Models;

namespace Whisperwood.Services
{
    public class AuthService : IAuthService
    {
        private readonly WhisperwoodDbContext dbContext;
        private readonly UserManager<Users> userManager;
        private readonly SignInManager<Users> signInManager;
        private readonly JwtService jwtService;

        public AuthService(WhisperwoodDbContext dbContext, UserManager<Users> userManager, SignInManager<Users> signInManager, JwtService jwtService)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.jwtService = jwtService;
        }

        public async Task<IActionResult> RegisterUserAsync(UserDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
            {
                return new BadRequestObjectResult(new { message = "Passwords do not match" });
            }

            var user = new Users
            {
                UserName = dto.Username,
                Name = dto.Name!,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                ImageURL = dto.ImageURL,
            };

            var result = await userManager.CreateAsync(user, dto.Password!);
            if (!result.Succeeded)
            {
                return new BadRequestObjectResult(result.Errors);
            }

            var cart = new Cart { User = user };
            var wishlist = new Wishlist { User = user };
            dbContext.Cart.Add(cart);
            dbContext.Wishlist.Add(wishlist);
            await dbContext.SaveChangesAsync();

            return new OkObjectResult(new { message = "Registered Successfully!" });
        }

        public async Task<IActionResult> LoginUserAsync(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email!);
            if (user == null)
            {
                return new UnauthorizedObjectResult(new { message = "Invalid email or password" });
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password!, true);
            if (!result.Succeeded)
            {
                return new UnauthorizedObjectResult(new { message = "Invalid email or password" });
            }

            return new OkObjectResult(new
            {
                Message = "Login Success",
                Token = jwtService.GenerateToken(user)
            });
        }

        public async Task<IActionResult> GetUserByIdAsync(Guid userId)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return new NotFoundObjectResult(new { message = "User not found" });
            }

            var dto = new UserResponseDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ImageURL = user.ImageURL,
                MembershipId = user.MembershipId,
                OrdersCount = user.OrdersCount,
                IsAdmin = user.IsAdmin,
                IsStaff = user.IsStaff,
                IsActive = user.IsActive
            };

            return new OkObjectResult(dto);
        }


    }
}
