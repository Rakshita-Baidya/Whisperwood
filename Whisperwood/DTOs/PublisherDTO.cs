using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class PublisherDTO
    {
        [Required]
        public required string Name { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Address { get; set; }
        [Phone]
        public string? Contact { get; set; }
    }

    public class PublisherUpdateDTO
    {
        public string? Name { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Address { get; set; }
        [Phone]
        public string? Contact { get; set; }
    }
}
