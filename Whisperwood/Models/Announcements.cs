using System.ComponentModel.DataAnnotations;

namespace Whisperwood.Models
{
    public class Announcements
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string? Title { get; set; }
        public string? Message { get; set; }
        [Required]
        public DateOnly StartDate { get; set; }
        [Required]
        public DateOnly EndDate { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public Users Users { get; set; }
    }
}
