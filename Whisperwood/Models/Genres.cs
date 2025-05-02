using System.ComponentModel.DataAnnotations;

namespace Whisperwood.Models
{
    public class Genres
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string? Name { get; set; }
        public string? Description { get; set; }
        public ICollection<GenreBooks> GenreBooks { get; set; } = [];

    }
}
