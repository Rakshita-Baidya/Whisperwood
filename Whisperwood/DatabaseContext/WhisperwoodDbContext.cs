using Microsoft.EntityFrameworkCore;
using Whisperwood.Models;

namespace Whisperwood.DatabaseContext
{
    public class WhisperwoodDbContext : DbContext
    {
        public WhisperwoodDbContext(DbContextOptions<WhisperwoodDbContext> options)
            : base(options) { }

        public DbSet<Announcements> Announcements { get; set; }
        public DbSet<AuthorBooks> AuthorBooks { get; set; }
        public DbSet<Authors> Authors { get; set; }
        public DbSet<Bill> Bill { get; set; }
        public DbSet<Books> Books { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<CartItem> CartItem { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<CategoryBooks> CategoryBooks { get; set; }
        public DbSet<CoverImages> CoverImages { get; set; }
        public DbSet<DiscountCode> DiscountCodes { get; set; }
        public DbSet<GenreBooks> GenreBooks { get; set; }
        public DbSet<Genres> Genres { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<PromotionBook> PromotionBook { get; set; }
        public DbSet<Promotions> Promotions { get; set; }
        public DbSet<PublisherBooks> PublisherBooks { get; set; }
        public DbSet<Publishers> Publishers { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Wishlist> Wishlist { get; set; }
        public DbSet<WishlistItem> WishlistItem { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.UserId);

            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithOne(u => u.Wishlist)
                .HasForeignKey<Wishlist>(w => w.UserId);

            modelBuilder.Entity<AuthorBooks>()
                .HasOne(ab => ab.Author)
                .WithMany(a => a.AuthorBooks)
                .HasForeignKey(ab => ab.AuthorId);

            modelBuilder.Entity<AuthorBooks>()
                .HasOne(ab => ab.Book)
                .WithMany(b => b.AuthorBooks)
                .HasForeignKey(ab => ab.BookId);

            modelBuilder.Entity<CategoryBooks>()
                .HasOne(cb => cb.Category)
                .WithMany(c => c.CategoryBooks)
                .HasForeignKey(cb => cb.CategoryId);

            modelBuilder.Entity<CategoryBooks>()
                .HasOne(cb => cb.Book)
                .WithMany(b => b.CategoryBooks)
                .HasForeignKey(cb => cb.BookId);

            modelBuilder.Entity<PublisherBooks>()
                .HasOne(pb => pb.Publisher)
                .WithMany(p => p.PublisherBooks)
                .HasForeignKey(pb => pb.PublisherId);

            modelBuilder.Entity<PublisherBooks>()
                .HasOne(pb => pb.Book)
                .WithMany(b => b.PublisherBooks)
                .HasForeignKey(pb => pb.BookId);

            modelBuilder.Entity<GenreBooks>()
                .HasOne(gb => gb.Genre)
                .WithMany(g => g.GenreBooks)
                .HasForeignKey(gb => gb.GenreId);

            modelBuilder.Entity<GenreBooks>()
                .HasOne(gb => gb.Book)
                .WithMany(b => b.GenreBooks)
                .HasForeignKey(gb => gb.BookId);

            modelBuilder.Entity<Announcements>()
                .HasOne(a => a.User)
                .WithMany(u => u.Announcements)
                .HasForeignKey(a => a.UserId);

        }

    }
}
