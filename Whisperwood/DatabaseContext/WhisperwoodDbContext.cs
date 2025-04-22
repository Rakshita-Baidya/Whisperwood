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
        public DbSet<Genres> Genre { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<PromotionBook> PromotionBook { get; set; }
        public DbSet<Promotions> Promotions { get; set; }
        public DbSet<PublisherBooks> PublisherBooks { get; set; }
        public DbSet<Publishers> Publishers { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Wishlist> Wishlist { get; set; }
        public DbSet<WishlistItem> WishlistItem { get; set; }

    }
}
