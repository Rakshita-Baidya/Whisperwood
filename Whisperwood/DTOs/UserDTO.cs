using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class UserDTO
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }
        public string? Username { get; set; }
        [Phone]
        public string? Contact { get; set; }
        public string? ImageURL { get; set; }
        [Required]
        public Guid MembershipId { get; set; }
        public bool IsEligibleForDiscount { get; set; }
        public int OrdersCount { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UserUpdateDTO
    {
        public string? Name { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Username { get; set; }
        [Phone]
        public string? Contact { get; set; }
        public string? ImageURL { get; set; }
        public Guid? MembershipId { get; set; }
        public bool? IsEligibleForDiscount { get; set; }
        public int? OrdersCount { get; set; }
        public bool? IsAdmin { get; set; }
        public bool? IsActive { get; set; }
    }
}