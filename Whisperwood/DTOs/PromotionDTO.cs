using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class PromotionDTO
    {
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
        [Precision(3, 2)]
        public decimal DiscountPercent { get; set; }
        [Required]
        public DateOnly StartDate { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }
        public Guid? UserId { get; set; }
    }

    public class PromotionUpdateDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        [Precision(3, 2)]
        public decimal? DiscountPercent { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public Guid? UserId { get; set; }
    }
}
