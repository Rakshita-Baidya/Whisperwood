using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Whisperwood.Models
{
    [PrimaryKey(nameof(BookId), nameof(CategoryId))]
    public class CategoryBooks
    {
        [ForeignKey("Book")]
        public Guid BookId { get; set; }
        public Books Book { get; set; }

        [ForeignKey("Category")]
        public Guid CategoryId { get; set; }
        public Categories Category { get; set; }
    }
}
