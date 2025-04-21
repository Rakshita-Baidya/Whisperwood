using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Whisperwood.Models
{
    [PrimaryKey(nameof(BookId), nameof(AuthorId))]
    public class AuthorBooks
    {
        [ForeignKey("Book")]
        public Guid BookId { get; set; }
        public Books Book { get; set; }

        [ForeignKey("Author")]
        public Guid AuthorId { get; set; }
        public Authors Author { get; set; }
    }
}
