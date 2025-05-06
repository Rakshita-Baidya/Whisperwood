using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class PromotionDto
    {
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }

        [Precision(5, 2)]
        [Range(0, 100, ErrorMessage = "Discount percent must be between 0 and 100.")]
        public required decimal DiscountPercent { get; set; }
        [Required]
        public required DateOnly StartDate { get; set; }
        [Required]
        public required DateOnly EndDate { get; set; }
    }

    public class PromotionUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }

        [Precision(5, 2)]
        [Range(0, 100, ErrorMessage = "Discount percent must be between 0 and 100.")]
        public decimal? DiscountPercent { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
