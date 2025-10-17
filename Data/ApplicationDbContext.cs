using Microsoft.EntityFrameworkCore;
using EasyGamesProject.Models; // Import models namespace for entity sets

namespace EasyGamesProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)  // Correctly call base constructor
        {
        }

        // DbSet properties represent tables in the database for each model entity
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Shop> Shops { get; set; }

        // DbSet for StockImages to represent stock item images
        public DbSet<StockImage> StockImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define relationship: ShoppingCart has one User (navigation), User can have many ShoppingCarts
            modelBuilder.Entity<ShoppingCart>()
                .HasOne(sc => sc.User)
                .WithMany() // No navigation property in User model currently
                .HasForeignKey(sc => sc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define relationship: ShoppingCart has one Stock, Stock can be in many ShoppingCarts
            modelBuilder.Entity<ShoppingCart>()
                .HasOne(sc => sc.Stock)
                .WithMany() // No navigation property in Stock model currently
                .HasForeignKey(sc => sc.StockId)
                .OnDelete(DeleteBehavior.Cascade);

            // Define relationship: Shop has one Proprietor (User), User can have many Shops
            modelBuilder.Entity<Shop>()
                .HasOne(s => s.Proprietor)
                .WithMany(u => u.Shops)  // Assumes User model has ICollection<Shop> Shops property
                .HasForeignKey(s => s.ProprietorId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete from User to Shops
        }
    }
}
