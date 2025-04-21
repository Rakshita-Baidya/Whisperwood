using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Whisperwood.Models
{
    [PrimaryKey(nameof(CartId), nameof(BookId))]
    public class CartItem
    {
        [ForeignKey("Cart")]
        public Guid CartId { get; set; }
        public Cart cart { get; set; }

        [ForeignKey("Book")]
        public Guid BookId { get; set; }
        public Books Book { get; set; }


        public required int Quantity { get; set; }
        [Precision(10, 2)]
        public decimal SubTotal { get; set; }

    }
}
