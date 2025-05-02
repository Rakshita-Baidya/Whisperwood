using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class OrderItemDto
    {
        public required Guid BookId { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public required int Quantity { get; set; }
    }

    public class OrderItemUpdateDto
    {
        public Guid? BookId { get; set; }

        [Range(1, int.MaxValue)]
        public int? Quantity { get; set; }
    }
}
