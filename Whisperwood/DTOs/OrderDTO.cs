using System.ComponentModel.DataAnnotations;
using static Whisperwood.Models.Orders;

namespace Whisperwood.DTOs
{
    public class OrderDto
    {
        [Required]
        public required Guid UserId { get; set; }
        [Required]
        public required List<OrderItemDto> Items { get; set; }
    }

    public class OrderUpdateDto
    {
        public Guid? UserId { get; set; }
        public OrderStatus? Status { get; set; }
        public List<OrderItemUpdateDto>? Items { get; set; }
    }
}