using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Whisperwood.Models
{
    public class Bill
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ClaimCode { get; set; } = Guid.NewGuid().ToString().Substring(0, 6);
        public DateOnly PickUpDate { get; set; }
        public string? PromoCode { get; set; }
        public Guid OrderId { get; set; }
        [JsonIgnore]
        public Orders Order { get; set; }
        [Precision(10, 2)]
        public decimal PromoDiscount { get; set; }
        [Precision(10, 2)]
        public decimal BulkDiscount { get; set; }
        [Precision(10, 2)]
        public decimal LoyalDiscount { get; set; }
        [Precision(10, 2)]
        public decimal BookDiscount { get; set; }
    }
}