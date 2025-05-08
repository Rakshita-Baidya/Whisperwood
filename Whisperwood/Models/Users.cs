using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Whisperwood.Models
{
    [Table("Users")]
    public class Users : IdentityUser<Guid>
    {
        public required string Name { get; set; }
        public string? ImageURL { get; set; }
        public Guid MembershipId { get; set; } = Guid.NewGuid();
        public bool IsEligibleForDiscount { get; set; } = false;
        public int OrdersCount { get; set; } = 0;
        public bool? IsAdmin { get; set; } = false;
        public bool? IsStaff { get; set; } = false;
        public bool? IsActive { get; set; } = true;
        [JsonIgnore]
        public ICollection<Announcements> Announcements { get; set; } = [];
        public Wishlist? Wishlist { get; set; }
        [JsonIgnore]
        public ICollection<Orders> Orders { get; set; } = [];
        [JsonIgnore]
        public ICollection<Promotions> Promotions { get; set; } = [];
        [JsonIgnore]
        public ICollection<Reviews> Reviews { get; set; } = [];
        public Cart? Cart { get; set; }
    }
}
