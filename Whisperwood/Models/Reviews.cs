using Microsoft.EntityFrameworkCore;

namespace Whisperwood.Models
{
    public class Reviews
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Users? Users { get; set; }
        public Guid BookId { get; set; }
        public Books Books { get; set; }
        [Precision(10, 2)]
        public decimal Rating { get; set; } = 0;
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }


    }
}
