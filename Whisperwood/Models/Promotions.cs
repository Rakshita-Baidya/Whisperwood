using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Whisperwood.Models
{
    public class Promotions
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string? Description { get; set; }
        public required string Code { get; set; } = Guid.NewGuid().ToString().Substring(0, 8);
        [Precision(5, 2)]
        [Range(0, 100, ErrorMessage = "Discount percent must be between 0 and 100.")]
        public decimal DiscountPercent { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public Guid? UserId { get; set; }
        public Users User { get; set; }

        public bool IsActive { get; set; } = true;

    }
}
