using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class UserDto
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public required string ConfirmPassword { get; set; }
        public string? Username { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        public string? ImageURL { get; set; }
    }

    public class UserUpdateDto
    {
        public string? Name { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Password { get; set; }
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string? ConfirmPassword { get; set; }
        public string? Username { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
        public string? ImageURL { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsActive { get; set; }
    }
    public class UserResponseDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string? PhoneNumber { get; set; }
        public string? ImageURL { get; set; }

        public Guid MembershipId { get; set; }
        public int OrdersCount { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsStaff { get; set; }
        public bool? IsActive { get; set; }
    }

}