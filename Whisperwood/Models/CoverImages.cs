using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Whisperwood.Models
{
    public class CoverImages
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string? CoverImageURL { get; set; }
        [JsonIgnore]
        public ICollection<Books> Books { get; set; } = [];

    }
}
