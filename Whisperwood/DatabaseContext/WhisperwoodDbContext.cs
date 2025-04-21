using Microsoft.EntityFrameworkCore;
using Whisperwood.Models;

namespace Whisperwood.DatabaseContext
{
    public class WhisperwoodDbContext : DbContext
    {
        public WhisperwoodDbContext(DbContextOptions<WhisperwoodDbContext> options)
            : base(options) { }

        public DbSet<Books> Books { get; set; }
        public DbSet<CoverImages> CoverImages { get; set; }
        public DbSet<Authors> Authors { get; set; }

    }
}
