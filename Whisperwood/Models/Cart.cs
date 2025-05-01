namespace Whisperwood.Models
{
    public class Cart
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Users User { get; set; }

    }
}
