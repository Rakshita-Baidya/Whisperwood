using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Whisperwood.Models
{
    public class Genres
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string? Name { get; set; }
        public string? Description { get; set; }
        [JsonIgnore]
        public ICollection<GenreBooks> GenreBooks { get; set; } = [];

    }
}
