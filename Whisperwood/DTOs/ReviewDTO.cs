using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class ReviewDTO
    {
        [Required]
        public required Guid BookId { get; set; }
        [Required]
        [Range(1, 5)]
        public required int Rating { get; set; }
        public string? Message { get; set; }
    }

    public class ReviewUpdateDto
    {
        public Guid? BookId { get; set; }
        [Range(1, 5)]
        public int? Rating { get; set; }
        public string? Message { get; set; }
    }
}
