using System.ComponentModel.DataAnnotations;

namespace Whisperwood.Models
{
    public class Authors
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Nationality { get; set; }

        public DateOnly DOB { get; set; }
        public string? Contact { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;

    }
}
