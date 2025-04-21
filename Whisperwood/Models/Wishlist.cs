namespace Whisperwood.Models
{
    public class Wishlist
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Users User { get; set; }

        public ICollection<WishlistItem> WishListItem { get; set; } = [];
    }
}
