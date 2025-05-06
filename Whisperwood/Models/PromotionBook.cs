using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Whisperwood.Models
{
    [PrimaryKey(nameof(BookId), nameof(PromotionId))]
    public class PromotionBook
    {
        [ForeignKey("Book")]
        public Guid BookId { get; set; }
        [JsonIgnore]

        public Books Book { get; set; }

        [ForeignKey("Promotion")]
        public Guid PromotionId { get; set; }
        public Promotions Promotion { get; set; }
    }
}
