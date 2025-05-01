using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class GenreDTO
    {
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
    }

    public class GenreUpdateDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}
