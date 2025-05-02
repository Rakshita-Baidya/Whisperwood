using Microsoft.EntityFrameworkCore;

namespace Whisperwood.DTOs
{
    public class DiscountCodeDto
    {
        public required string Code { get; set; }
        [Precision(3, 2)]
        public required decimal Percent { get; set; }
        public required DateOnly StartDate { get; set; }
        public required DateOnly EndDate { get; set; }
    }

    public class DiscountCodeUpdateDto
    {
        public string? Code { get; set; }
        [Precision(3, 2)]
        public decimal? Percent { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}
