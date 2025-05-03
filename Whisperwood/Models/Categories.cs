using System.Text.Json.Serialization;

namespace Whisperwood.Models
{
    public class Categories
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string? Name { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public ICollection<CategoryBooks> CategoryBooks { get; set; } = [];
    }
}
