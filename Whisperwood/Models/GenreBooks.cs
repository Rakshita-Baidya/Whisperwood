using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Whisperwood.Models
{
    [PrimaryKey(nameof(BookId), nameof(GenreId))]
    public class GenreBooks
    {
        [ForeignKey("Book")]
        public Guid BookId { get; set; }
        public Books Book { get; set; }

        [ForeignKey("Genre")]
        public Guid GenreId { get; set; }
        public Genres Genre { get; set; }
    }
}
