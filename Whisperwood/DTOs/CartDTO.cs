using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class CartDTO
    {
        [Required]
        public Guid UserId { get; set; }
        public List<CartItemDTO> CartItems { get; set; } = [];
    }

    public class CartUpdateDTO
    {
        public Guid? UserId { get; set; }
        public List<CartItemUpdateDTO>? CartItems { get; set; }
    }
}
