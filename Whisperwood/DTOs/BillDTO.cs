using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class BillDTO
    {
        [Required]
        public Guid ClaimCode { get; set; }
        [Required]
        public DateOnly PickUpDate { get; set; }
        [Required]
        public required string Status { get; set; }
        [Required]
        public Guid OrderId { get; set; }
    }

    public class BillUpdateDTO
    {
        public Guid? ClaimCode { get; set; }
        public DateOnly? PickUpDate { get; set; }
        public string? Status { get; set; }
        public Guid? OrderId { get; set; }
    }
}
