using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class DiscountCodeDTO
    {
        [Required]
        public required string Code { get; set; }
        [Precision(3, 2)]
        public decimal Percent { get; set; }
        [Required]
        public DateOnly StartDate { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }
    }

    public class DiscountCodeUpdateDTO
    {
        public string? Code { get; set; }
        [Precision(3, 2)]
        public decimal? Percent { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
