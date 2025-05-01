using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class CategoryDTO
    {
        [Required]
        public required string Name { get; set; }
        public string? Description { get; set; }
    }

    public class CategoryUpdateDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}