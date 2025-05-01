using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class CartItemDTO
    {
        [Required]
        public Guid CartId { get; set; }
        [Required]
        public Guid BookId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
        [Precision(10, 2)]
        public decimal SubTotal { get; set; }
    }

    public class CartItemUpdateDTO
    {
        public Guid? CartId { get; set; }
        public Guid? BookId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int? Quantity { get; set; }
        [Precision(10, 2)]
        public decimal? SubTotal { get; set; }
    }
}