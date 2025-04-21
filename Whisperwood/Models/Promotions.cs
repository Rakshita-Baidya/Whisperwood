using Microsoft.EntityFrameworkCore;

namespace Whisperwood.Models
{
    public class Promotions
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        [Precision(3, 2)]
        public decimal DiscountPercent { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public Guid? UserId { get; set; }
        public Users Users { get; set; }
    }
}
