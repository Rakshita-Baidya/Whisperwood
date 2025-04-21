using Microsoft.EntityFrameworkCore;

namespace Whisperwood.Models
{
    public class DiscountCode
    {
        public Guid Id { get; set; }
        public required string Code { get; set; }
        [Precision(3, 2)]
        public decimal Percent { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
