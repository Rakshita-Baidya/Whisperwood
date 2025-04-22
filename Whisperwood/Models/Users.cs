namespace Whisperwood.Models
{
    public class Users
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public string Password { get; set; }
        public string? Username { get; set; }
        public string? Contact { get; set; }

        public string? ImageURL { get; set; }
        public Guid MembershipId { get; set; } = Guid.NewGuid();
        public bool IsEligibleForDiscount { get; set; } = false;
        public int OrdersCount { get; set; } = 0;
        public bool? IsAdmin { get; set; } = false;

        public ICollection<Announcements> Announcements { get; set; } = [];
        public Wishlist Wishlist { get; set; }
        public ICollection<Orders> Orders { get; set; } = [];

    }
}
