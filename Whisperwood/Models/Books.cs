using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Whisperwood.Models
{
    [Table("Books")]
    [Index(nameof(ISBN), IsUnique = true)]
    public class Books
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string? Title { get; set; }

        [Required]
        [StringLength(13)]
        public string? ISBN { get; set; }

        [Precision(10, 2)]
        public decimal Price { get; set; }

        public string? Synopsis { get; set; }

        public CoverImages? CoverImage { get; set; }

        [Precision(1, 2)]
        public decimal AverageRating { get; set; } = decimal.Zero;

        [Required]
        public DateTime PublishedDate { get; set; } = DateTime.Now;

        [Required]
        public int Stock { get; set; }

        public int SalesCount { get; set; } = 0;
        public string? Language { get; set; }
        public string? Format { get; set; }
        public int? Edition { get; set; }
        public bool AvailablilityStatus { get; set; } = true;
        public ICollection<AuthorBooks> AuthorBooks { get; set; } = [];
        public ICollection<PublisherBooks> PublisherBooks { get; set; } = [];
        public ICollection<CategoryBooks> CategoryBooks { get; set; } = [];

        public DateTime DateAdded { get; set; } = DateTime.Now;

    }
}
