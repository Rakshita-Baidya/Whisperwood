using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class PublisherBookDTO
    {
        [Required]
        public Guid BookId { get; set; }
        [Required]
        public Guid PublisherId { get; set; }
    }

    public class PublisherBookUpdateDTO
    {
        public Guid? BookId { get; set; }
        public Guid? PublisherId { get; set; }
    }
}
