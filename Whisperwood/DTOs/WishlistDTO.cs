using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class WishlistDTO
    {
        [Required]
        public Guid UserId { get; set; }

        public List<Guid> BookId { get; set; } = [];
    }

    public class WishlistUpdateDTO
    {
        public Guid? UserId { get; set; }
        public List<Guid>? BookId { get; set; }
    }
}
