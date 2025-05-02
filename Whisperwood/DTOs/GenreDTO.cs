using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class GenreDto
    {
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
    }

    public class GenreUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
