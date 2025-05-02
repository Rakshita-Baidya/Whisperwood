using Microsoft.EntityFrameworkCore;

namespace Whisperwood.Models
{
    public class DiscountCode
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Code { get; set; } = Guid.NewGuid().ToString().Substring(0, 8);
        [Precision(3, 2)]
        public decimal Percent { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
    }
}
