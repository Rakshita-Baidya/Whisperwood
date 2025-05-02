using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class CoverImageDto
    {
        [Required]
        public required string CoverImageURL { get; set; }
    }

    public class CoverImageUpdateDto
    {
        public string? CoverImageURL { get; set; }
    }
}
