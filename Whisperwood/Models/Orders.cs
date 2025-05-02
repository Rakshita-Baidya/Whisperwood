using Microsoft.EntityFrameworkCore;

namespace Whisperwood.Models
{
    public class Orders
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Users Users { get; set; }
        public DateOnly Date { get; set; } = new DateOnly();
        [Precision(10, 2)]
        public decimal SubTotal { get; set; }
        [Precision(10, 2)]
        public decimal TotalAmount { get; set; }
        public enum OrderStatus { Pending, Cancelled, Fulfilled }
        public OrderStatus Status { get; set; }

        public Guid DiscountCodeId { get; set; }
        public DiscountCode DiscountCode { get; set; }

        public Bill? Bill { get; set; }

    }
}
