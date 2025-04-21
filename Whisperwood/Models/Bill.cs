namespace Whisperwood.Models
{
    public class Bill
    {
        public Guid Id { get; set; }
        public Guid ClaimCode { get; set; }
        public DateOnly PickUpDate { get; set; }
        public string Status { get; set; }
        public Guid OrderId { get; set; }
        public Orders Order { get; set; }
    }
}
