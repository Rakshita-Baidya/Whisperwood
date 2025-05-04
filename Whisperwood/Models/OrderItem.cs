using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Whisperwood.Models
{
    [PrimaryKey(nameof(OrderId), nameof(BookId))]
    public class OrderItem
    {
        [ForeignKey("Order")]
        public Guid OrderId { get; set; }
        public Orders Order { get; set; }

        [ForeignKey("Book")]
        public Guid BookId { get; set; }
        public Books Book { get; set; }

        [Precision(10, 2)]
        public decimal UnitPrice { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public required int Quantity { get; set; }

        [Precision(10, 2)]
        public decimal SubTotal { get; set; }
    }
}
