using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class CategoryBookDTO
    {
        [Required]
        public Guid BookId { get; set; }
        [Required]
        public Guid CategoryId { get; set; }
    }

    public class CategoryBookUpdateDTO
    {
        public Guid? BookId { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
