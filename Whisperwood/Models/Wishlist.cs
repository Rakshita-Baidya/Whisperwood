using System.Text.Json.Serialization;

namespace Whisperwood.Models
{
    public class Wishlist
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [JsonIgnore]
        public Users User { get; set; }
        public ICollection<WishlistItem> WishListItems { get; set; } = [];
    }
}
