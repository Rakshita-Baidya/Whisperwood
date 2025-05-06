using Microsoft.EntityFrameworkCore;

namespace Whisperwood.Models
{
    public class Bill
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ClaimCode { get; set; } = Guid.NewGuid().ToString().Substring(0, 6);
        public DateOnly PickUpDate { get; set; }
        public string? PromoCode { get; set; }
        public Guid OrderId { get; set; }
        public Orders Order { get; set; }
        [Precision(10, 2)]
        public decimal PromoDiscount { get; set; }
    }
}
