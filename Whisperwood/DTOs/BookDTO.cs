using System.ComponentModel.DataAnnotations;
using static Whisperwood.Models.Books;

namespace Whisperwood.DTOs
{
    public class BookDto
    {
        [Required]
        public required string Title { get; set; }
        [Required]
        [StringLength(13)]
        public required string ISBN { get; set; }
        [Required]
        public required decimal Price { get; set; }
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
        public required List<Guid> AuthorIds { get; set; }
        [Required]
        public required List<Guid> GenreIds { get; set; }
        [Required]
        public required List<Guid> CategoryIds { get; set; }
        [Required]
        public required List<Guid> PublisherIds { get; set; }
    }

    public class BookUpdateDto
    {
        public string? Title { get; set; }
        public string? ISBN { get; set; }
        public decimal? Price { get; set; }
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
}