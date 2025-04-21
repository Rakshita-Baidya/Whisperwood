using System.ComponentModel.DataAnnotations;

namespace Whisperwood.Models
{
    public class Genres
    {
        [Key]
        public Guid Id { get; set; }
        public required string? Name { get; set; }
        public string? Description { get; set; }
    }
}
