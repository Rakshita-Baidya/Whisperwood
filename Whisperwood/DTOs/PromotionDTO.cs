using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class PromotionDto
    {
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
        [Required]
        public required decimal DiscountPercent { get; set; }
        [Required]
        public required DateOnly StartDate { get; set; }
        [Required]
        public required DateOnly EndDate { get; set; }
        [Required]
        public required List<Guid> BookIds { get; set; }
    }

    public class PromotionUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? DiscountPercent { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public List<Guid>? BookIds { get; set; }
    }
}
