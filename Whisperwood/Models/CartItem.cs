using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Whisperwood.Models
{
    [PrimaryKey(nameof(CartId), nameof(BookId))]
    public class CartItem
    {
        [ForeignKey("Cart")]
        public Guid CartId { get; set; }
        [JsonIgnore]
        public Cart cart { get; set; }

        [ForeignKey("Book")]
        public Guid BookId { get; set; }
        public Books Book { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public required int Quantity { get; set; }
        [Precision(10, 2)]
        public decimal SubTotal { get; set; }

    }
}
