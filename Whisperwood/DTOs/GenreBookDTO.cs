using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class GenreBookDTO
    {
        [Required]
        public Guid BookId { get; set; }
        [Required]
        public Guid GenreId { get; set; }
    }

    public class GenreBookUpdateDTO
    {
        public Guid? BookId { get; set; }
        public Guid? GenreId { get; set; }
    }
}
