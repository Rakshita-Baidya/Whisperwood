using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Whisperwood.Models
{
    public class Authors
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string? Name { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Nationality { get; set; }

        public DateOnly DOB { get; set; }
        [Phone]
        public string? Contact { get; set; }
        [JsonIgnore]
        public ICollection<AuthorBooks> AuthorBooks { get; set; } = [];

    }
}
