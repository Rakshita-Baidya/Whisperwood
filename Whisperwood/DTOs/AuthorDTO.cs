using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class AuthorDto
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        public string? Address { get; set; }
        public string? Nationality { get; set; }
        [Required]
        public required DateOnly DOB { get; set; }
        [Phone]
        public string? Contact { get; set; }
    }

    public class AuthorUpdateDto
    {
        public string? Name { get; set; }
        [EmailAddress]
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Nationality { get; set; }
        public DateOnly? DOB { get; set; }
        [Phone]
        public string? Contact { get; set; }
    }
}