using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class CategoryDto
    {
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
    }

    public class CategoryUpdateDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}