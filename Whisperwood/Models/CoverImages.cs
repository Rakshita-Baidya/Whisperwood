using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Whisperwood.Models
{
    public class CoverImages
    {
        [Key, ForeignKey("Book")]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string? CoverImageURL { get; set; }
        public Guid BookId { get; set; }
        public Books Book { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;


    }
}
