using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class CoverImageDTO
    {
        [Required]
        public required string CoverImageURL { get; set; }
        [Required]
        public Guid BookId { get; set; }
    }

    public class CoverImageUpdateDTO
    {
        public string? CoverImageURL { get; set; }
        public Guid? BookId { get; set; }
    }
}
