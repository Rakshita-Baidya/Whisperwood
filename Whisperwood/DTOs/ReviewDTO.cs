using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class ReviewDTO
    {
        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid BookId { get; set; }
        [Range(0, 5)]
        public int Rating { get; set; }
        [Required]
        public required string Message { get; set; }
    }

    public class ReviewUpdateDTO
    {
        public Guid? UserId { get; set; }
        public Guid? BookId { get; set; }
        [Range(0, 5)]
        public int? Rating { get; set; }
        public string? Message { get; set; }
    }
}
