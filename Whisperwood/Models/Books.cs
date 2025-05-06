using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        [Required]
        [Precision(10, 2)]
        public decimal Price { get; set; }
        public string? Synopsis { get; set; }
        public Guid? CoverImageId { get; set; }
        public CoverImages? CoverImage { get; set; }

        [Precision(3, 2)]
        public decimal AverageRating { get; set; } = decimal.Zero;

        [Required]
        public DateOnly PublishedDate { get; set; }

        [Required]
        public int Stock { get; set; }

        public int SalesCount { get; set; } = 0;
        public string? Language { get; set; }
        public BookFormat Format { get; set; }

        public int? Edition { get; set; }
        public bool AvailabilityStatus { get; set; } = true;
        public ICollection<AuthorBooks> AuthorBooks { get; set; } = [];
        public ICollection<PublisherBooks> PublisherBooks { get; set; } = [];
        public ICollection<CategoryBooks> CategoryBooks { get; set; } = [];
        public ICollection<GenreBooks> GenreBooks { get; set; } = [];
        public ICollection<PromotionBook> PromotionBooks { get; set; } = [];
        [JsonIgnore]
        public ICollection<WishlistItem> WishlistItems { get; set; } = [];
        [JsonIgnore]
        public ICollection<OrderItem> OrderItems { get; set; } = [];
        [JsonIgnore]
        public ICollection<Reviews> Reviews { get; set; } = [];
        [JsonIgnore]
        public ICollection<CartItem> CartItems { get; set; } = [];
        public enum BookFormat
        {
            Paperback,
            Hardcover,
            SignedEdition,
            LimitedEdition,
            FirstEdition,
            CollectorsEdition,
            AuthorsEdition,
            DeluxeEdition,
            Ebook,
            Audiobook
        }

    }
}
