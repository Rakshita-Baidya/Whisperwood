using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class WishlistItemDTO
    {
        [Required]
        public Guid WishlistId { get; set; }
        [Required]
        public Guid BookId { get; set; }
    }

    public class WishlistItemUpdateDTO
    {
        public Guid? WishlistId { get; set; }
        public Guid? BookId { get; set; }
    }
}