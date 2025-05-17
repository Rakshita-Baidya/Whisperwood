using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                ImageURL = "https://i.imgur.com/L8yG19z.jpeg",
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

        public async Task<IActionResult> GetUserAsync(Guid userId)
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

        public async Task<IActionResult> GetUserByIdAsync(Guid loggedInUserId, Guid userId)
        {
            var loggedInUser = await dbContext.Users.FindAsync(loggedInUserId);
            if (loggedInUser == null)
            {
                return new UnauthorizedObjectResult(new { message = "Authenticated user not found." });
            }

            // Only admins can view other users' profiles
            if (userId != loggedInUserId && !loggedInUser.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult(new { message = "Only admins can view other users' profiles." });
            }

            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return new NotFoundObjectResult(new { message = "User not found!" });
            }

            var userDto = new UserResponseDto
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

            return new OkObjectResult(userDto);
        }
        public async Task<IActionResult> GetAllUsersAsync(Guid userId)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult(new { message = "Only admins can view all users." });
            }

            var users = await dbContext.Users
                .Select(u => new UserResponseDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Name = u.Name,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    ImageURL = u.ImageURL,
                    MembershipId = u.MembershipId,
                    OrdersCount = u.OrdersCount,
                    IsAdmin = u.IsAdmin,
                    IsStaff = u.IsStaff,
                    IsActive = u.IsActive
                })
                .ToListAsync();

            return new OkObjectResult(users);
        }

        public async Task<IActionResult> UpdateUserAsync(Guid loggedInUserId, Guid userId, UserUpdateDto dto)
        {
            var loggedInUser = await dbContext.Users.FindAsync(loggedInUserId);
            if (loggedInUser == null)
            {
                return new UnauthorizedObjectResult(new { message = "Authenticated user not found." });
            }

            bool isAdmin = loggedInUser.IsAdmin.GetValueOrDefault(false);
            if (userId != loggedInUserId && !isAdmin)
            {
                return new UnauthorizedObjectResult(new { message = "Only admins can update other users." });
            }

            var user = await dbContext.Users.FindAsync(userId);
            if (user == null)
            {
                return new NotFoundObjectResult(new { message = "User not found!" });
            }

            // Update fields if provided
            if (dto.Name != null) user.Name = dto.Name;
            if (dto.Email != null) user.Email = dto.Email;
            if (dto.Username != null) user.UserName = dto.Username;
            if (dto.PhoneNumber != null) user.PhoneNumber = dto.PhoneNumber;
            if (dto.ImageURL != null) user.ImageURL = dto.ImageURL;

            // Admin-only fields
            if (isAdmin)
            {
                if (dto.IsAdmin.HasValue) user.IsAdmin = dto.IsAdmin.Value;
                if (dto.IsStaff.HasValue) user.IsStaff = dto.IsStaff.Value;
                if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;
            }

            await dbContext.SaveChangesAsync();
            return new OkObjectResult(new { message = "Profile updated successfully" });
        }
    }
}
