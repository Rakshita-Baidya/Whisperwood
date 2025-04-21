namespace Whisperwood.Models
{
    public class Categories
    {
        public Guid Id { get; set; }
        public required string? Name { get; set; }
        public string Description { get; set; }
        public ICollection<CategoryBooks> CategoryBooks { get; set; } = [];


    }
}
