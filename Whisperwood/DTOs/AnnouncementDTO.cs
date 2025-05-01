using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class AnnouncementDTO
    {
        [Required]
        public required string Title { get; set; }

        public string? Message { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        public Guid? UserId { get; set; }
    }

    public class AnnouncementUpdateDTO
    {
        public string? Title { get; set; }
        public string? Message { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public Guid? UserId { get; set; }
    }
}