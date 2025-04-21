using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Whisperwood.Models
{
    [PrimaryKey(nameof(BookId), nameof(PublisherId))]
    public class PublisherBooks
    {
        [ForeignKey("Book")]
        public Guid BookId { get; set; }
        public Books Book { get; set; }

        [ForeignKey("Publisher")]
        public Guid PublisherId { get; set; }
        public Publishers Publisher { get; set; }
    }
}
