using System.ComponentModel.DataAnnotations;
using Whisperwood.Models;
using static Whisperwood.Models.Books;

namespace Whisperwood.DTOs
{
    public class BookDto
    {
        [Required]
        public required string Title { get; set; }
        [Required]
        [StringLength(13, MinimumLength = 10)]
        [RegularExpression(@"^\d{10}(\d{3})?$", ErrorMessage = "ISBN must be 10 or 13 digits.")]
        public required string ISBN { get; set; }
        [Required]
        public required decimal Price { get; set; }
        public decimal DiscountPercentage { get; set; } = 0;
        public bool IsOnSale { get; set; } = false;
        public DateOnly? DiscountStartDate { get; set; }
        public DateOnly? DiscountEndDate { get; set; }
        public string? Synopsis { get; set; }
        public Guid? CoverImageId { get; set; }
        [Required]
        public required DateOnly PublishedDate { get; set; }
        [Required]
        public required int Stock { get; set; }
        public string? Language { get; set; }
        public BookFormat? Format { get; set; }
        public int? Edition { get; set; }
        [Required]
        public required List<Guid>? AuthorIds { get; set; }
        [Required]
        public required List<Guid>? GenreIds { get; set; }
        [Required]
        public required List<Guid>? CategoryIds { get; set; }
        [Required]
        public required List<Guid>? PublisherIds { get; set; }
    }

    public class BookUpdateDto
    {
        public string? Title { get; set; }
        [StringLength(13, MinimumLength = 10)]
        [RegularExpression(@"^\d{10}(\d{3})?$", ErrorMessage = "ISBN must be 10 or 13 digits.")]
        public string? ISBN { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPercentage { get; set; } = 0;
        public bool? IsOnSale { get; set; } = false;
        public DateOnly? DiscountStartDate { get; set; }
        public DateOnly? DiscountEndDate { get; set; }
        public string? Synopsis { get; set; }
        public Guid? CoverImageId { get; set; }
        public DateOnly? PublishedDate { get; set; }
        public int? Stock { get; set; }
        public string? Language { get; set; }
        public BookFormat? Format { get; set; }
        public int? Edition { get; set; }
        public List<Guid>? AuthorIds { get; set; }
        public List<Guid>? GenreIds { get; set; }
        public List<Guid>? CategoryIds { get; set; }
        public List<Guid>? PublisherIds { get; set; }
    }

    public class BookFilterDto
    {
        // Filters
        public List<Guid>? AuthorIds { get; set; }
        public List<Guid>? GenreIds { get; set; }
        public bool? IsAvailable { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public bool? IsOnSale { get; set; } = false;
        public decimal? MinRating { get; set; }
        public string? Language { get; set; }
        public List<Books.BookFormat>? Formats { get; set; }
        public List<Guid>? PublisherIds { get; set; }
        public List<Guid>? CategoryIds { get; set; }

        // Search
        public string? SearchTerm { get; set; }

        // Sorting
        public SortByOption? SortBy { get; set; }
        public SortOrders? SortOrder { get; set; }

        public enum SortByOption { Title, PublicationDate, Price, Popularity }

        public enum SortOrders { Ascending, Descending }
    }
}