using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Whisperwood.Models
{
    [Index(nameof(UserId))]
    [Index(nameof(Status))]
    [Index(nameof(OrderedAt))]
    public class Orders
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Users User { get; set; }
        [Precision(10, 2)]
        public decimal SubTotal { get; set; }
        [Precision(10, 2)]
        public string? PromoCode { get; set; }
        public decimal Discount { get; set; } = 0;
        [Precision(10, 2)]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "varchar(20)")]
        public OrderStatus Status { get; set; }
        public DateOnly Date { get; set; } = new DateOnly();
        public DateTime OrderedAt { get; set; } = DateTime.UtcNow;
        public Bill? OrderBill { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public enum OrderStatus { Pending, Cancelled, Fulfilled }

    }
}
