using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class AnnouncementDto
    {
        [Required]
        public required string Title { get; set; }
        public string? Message { get; set; }
        [Required]
        public required DateOnly StartDate { get; set; }
        [Required]
        public required DateOnly EndDate { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "At least one recipient group must be selected.")]
        public required List<string> RecipientGroups { get; set; }
    }

    public class AnnouncementUpdateDto
    {
        public string? Title { get; set; }
        public string? Message { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public List<string>? RecipientGroups { get; set; }
    }
}