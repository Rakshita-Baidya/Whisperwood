using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Whisperwood.Models
{
    public class Publishers
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        [Phone]
        public string? Contact { get; set; }
        [JsonIgnore]
        public ICollection<PublisherBooks> PublisherBooks { get; set; } = [];

    }
}
