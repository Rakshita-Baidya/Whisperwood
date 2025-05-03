using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Models;
using Whisperwood.Services;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly WhisperwoodDbContext dbContext;
        private readonly UserManager<Users> userManager;
        private readonly SignInManager<Users> signInManager;
        private readonly JwtService jwtService;

        public AuthController(WhisperwoodDbContext dbContext, UserManager<Users> userManager, SignInManager<Users> signInManager, JwtService jwtService)
        {
            this.dbContext = dbContext;
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(UserDto dto)
        {
            Users user = new Users
            {
                UserName = dto.Username,
                Name = dto.Name!,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                ImageURL = dto.ImageURL,
            };

            var result = await userManager.CreateAsync(user, dto.Password!);
            var cart = new Cart { User = user };
            var wishlist = new Wishlist { User = user };
            dbContext.Cart.Add(cart);
            dbContext.Wishlist.Add(wishlist);
            await dbContext.SaveChangesAsync();

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok("Registered Successfully!");
        }


        [HttpPost("login")]
        public async Task<IActionResult> LoginUser(LoginDto loginDto)
        {
            var user = await userManager.FindByEmailAsync(loginDto.Email!);
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }
            var result = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password!, true);
            if (!result.Succeeded)
            {
                return Unauthorized("Invalid email or password");

            }

            if (result.Succeeded)
            {
                return Ok(
                    new
                    {
                        Message = "Login Success",
                        Token = jwtService.GenerateToken(user)
                    }
                );
            }
            return Unauthorized("Wrong Password");
        }
    }
}
