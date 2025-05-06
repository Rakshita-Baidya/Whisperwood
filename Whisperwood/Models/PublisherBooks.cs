using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Whisperwood.Models
{
    [PrimaryKey(nameof(BookId), nameof(PublisherId))]
    public class PublisherBooks
    {
        [ForeignKey("Book")]
        public Guid BookId { get; set; }
        [JsonIgnore]

        public Books Book { get; set; }

        [ForeignKey("Publisher")]
        public Guid PublisherId { get; set; }
        public Publishers Publisher { get; set; }
    }
}
