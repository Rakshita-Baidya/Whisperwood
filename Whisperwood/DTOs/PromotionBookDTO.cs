using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class PromotionBookDTO
    {
        [Required]
        public Guid BookId { get; set; }
        [Required]
        public Guid PromotionId { get; set; }
    }

    public class PromotionBookUpdateDTO
    {
        public Guid? BookId { get; set; }
        public Guid? PromotionId { get; set; }
    }
}