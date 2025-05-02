using Microsoft.AspNetCore.Identity;

namespace Whisperwood.Models
{
    public class Users : IdentityUser<long>
    {
        public required string Name { get; set; }
        public string? Username { get; set; }
        public string? Contact { get; set; }
        public string? ImageURL { get; set; }
        public Guid MembershipId { get; set; } = Guid.NewGuid();
        public bool IsEligibleForDiscount { get; set; } = false;
        public int OrdersCount { get; set; } = 0;
        public bool? IsAdmin { get; set; } = false;
        public bool? IsActive { get; set; } = true;

        public ICollection<Announcements> Announcements { get; set; } = [];
        public Wishlist? Wishlist { get; set; }
        public ICollection<Orders> Orders { get; set; } = [];

        public Cart? Cart { get; set; }
    }
}
