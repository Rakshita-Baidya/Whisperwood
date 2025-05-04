using System.Text.Json.Serialization;

namespace Whisperwood.Models
{
    public class Cart
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        [JsonIgnore]
        public Users User { get; set; }
        [JsonIgnore]
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    }
}
