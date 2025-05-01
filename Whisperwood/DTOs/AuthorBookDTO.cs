using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class AuthorBookDTO
    {
        [Required]
        public Guid BookId { get; set; }
        [Required]
        public Guid AuthorId { get; set; }
    }

    public class AuthorBookUpdateDTO
    {
        public Guid? BookId { get; set; }
        public Guid? AuthorId { get; set; }
    }
}