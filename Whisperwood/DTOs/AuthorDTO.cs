using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class AuthorDTO
    {
        [Required]
        public required string Name { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }
        public string? Address { get; set; }
        public string? Nationality { get; set; }
        public DateOnly DOB { get; set; }
        [Phone]
        public string? Contact { get; set; }
    }

    public class AuthorUpdateDTO
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