using System.Text.Json.Serialization;

namespace Whisperwood.Models
{
    public class Bill
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ClaimCode { get; set; } = Guid.NewGuid().ToString().Substring(0, 6);
        public DateOnly PickUpDate { get; set; }
        public string Status { get; set; }
        public Guid OrderId { get; set; }
        [JsonIgnore]
        public Orders Order { get; set; }
    }
}
