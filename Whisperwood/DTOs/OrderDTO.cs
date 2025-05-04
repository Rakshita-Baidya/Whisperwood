using static Whisperwood.Models.Orders;

namespace Whisperwood.DTOs
{
    public class OrderUpdateDto
    {
        public OrderStatus? Status { get; set; }
    }
}