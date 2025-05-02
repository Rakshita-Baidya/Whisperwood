using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class DiscountCodeDto
    {
        [Precision(5, 2)]
        [Range(0, 100, ErrorMessage = "Discount percent must be between 0 and 100.")]
        public required decimal Percent { get; set; }
        public required DateOnly StartDate { get; set; }
        public required DateOnly EndDate { get; set; }
    }

    public class DiscountCodeUpdateDto
    {
        [Precision(5, 2)]
        [Range(0, 100, ErrorMessage = "Discount percent must be between 0 and 100.")]
        public decimal? Percent { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
