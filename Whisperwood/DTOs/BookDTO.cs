using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Whisperwood.DTOs
{
    public class BookDTO
    {
        [Required]
        public required string Title { get; set; }
        [Required]
        [StringLength(13)]
        public required string ISBN { get; set; }
        [Precision(10, 2)]
        public decimal Price { get; set; }
        public string? Synopsis { get; set; }
        public Guid? CoverImageId { get; set; }
        [Precision(1, 2)]
        public decimal AverageRating { get; set; }
        [Required]
        public DateOnly PublishedDate { get; set; }
        [Required]
        public int Stock { get; set; }
        public int SalesCount { get; set; }
        public string? Language { get; set; }
        public string? Format { get; set; }
        public int? Edition { get; set; }
        public bool AvailabilityStatus { get; set; }

        public List<Guid> AuthorId { get; set; } = [];
        public List<Guid> CategoryId { get; set; } = [];
        public List<Guid> GenreId { get; set; } = [];
        public List<Guid> PublisherId { get; set; } = [];
        public List<Guid> PromotionId { get; set; } = [];
    }

    public class BookUpdateDTO
    {
        public string? Title { get; set; }
        [StringLength(13)]
        public string? ISBN { get; set; }
        [Precision(10, 2)]
        public decimal? Price { get; set; }
        public string? Synopsis { get; set; }
        public Guid? CoverImageId { get; set; }
        [Precision(1, 2)]
        public decimal? AverageRating { get; set; }
        public DateOnly? PublishedDate { get; set; }
        [Required]
        public int? Stock { get; set; }
        public int? SalesCount { get; set; }
        public string? Language { get; set; }
        public string? Format { get; set; }
        public int? Edition { get; set; }
        public bool? AvailabilityStatus { get; set; }
        public List<Guid>? AuthorId { get; set; }
        public List<Guid>? CategoryId { get; set; }
        public List<Guid>? GenreId { get; set; }
        public List<Guid>? PublisherId { get; set; }
        public List<Guid>? PromotionId { get; set; }
    }
}