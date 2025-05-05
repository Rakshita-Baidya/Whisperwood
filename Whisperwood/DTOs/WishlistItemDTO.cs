using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class WishlistItemDTO
    {
        [Required]
        public Guid BookId { get; set; }
    }
}