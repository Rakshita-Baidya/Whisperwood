namespace Whisperwood.Models
{
    public class Announcements
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public Guid? UserId { get; set; }
        public Users Users { get; set; }
    }
}
