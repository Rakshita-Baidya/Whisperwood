using System.ComponentModel.DataAnnotations;

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
        public string? Contact { get; set; }
        public ICollection<PublisherBooks> PublisherBooks { get; set; } = [];

        public DateTime DateAdded { get; set; } = DateTime.Now;

    }
}
