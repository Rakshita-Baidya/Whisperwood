using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class OrderDTO
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public DateOnly Date { get; set; }
        [Precision(10, 2)]
        public decimal SubTotal { get; set; }
        [Precision(10, 2)]
        public decimal TotalAmount { get; set; }
        [Required]
        public required string Status { get; set; }
        [Required]
        public Guid DiscountCodeId { get; set; }
    }

    public class OrderUpdateDTO
    {
        public Guid? UserId { get; set; }
        public DateOnly? Date { get; set; }
        [Precision(10, 2)]
        public decimal? SubTotal { get; set; }
        [Precision(10, 2)]
        public decimal? TotalAmount { get; set; }
        public string? Status { get; set; }
        public Guid? DiscountCodeId { get; set; }
    }
}