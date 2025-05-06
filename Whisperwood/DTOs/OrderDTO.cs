using static Whisperwood.Models.Orders;

namespace Whisperwood.DTOs
{
    public class OrderDto
    {
        public string? PromoCode { get; set; }
    }
    public class OrderUpdateDto
    {
        public OrderStatus? Status { get; set; }
        public string? ClaimCode { get; set; }
    }
}