using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Whisperwood.Models
{
    [PrimaryKey(nameof(BookId), nameof(WishlistId))]
    public class WishlistItem
    {
        [ForeignKey("Wishlist")]
        public Guid WishlistId { get; set; }
        [JsonIgnore]

        public Wishlist Wishlist { get; set; }

        [ForeignKey("Book")]
        public Guid BookId { get; set; }
        public Books Book { get; set; }
    }
}
