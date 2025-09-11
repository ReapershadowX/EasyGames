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

        // Configure entity relationships and behaviors using Fluent API
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Define relationship: ShoppingCart has one User (navigation), User can have many ShoppingCarts (if you define navigation in User)
            modelBuilder.Entity<ShoppingCart>()
                .HasOne(sc => sc.User)
                .WithMany() // No navigation property in User model currently
                .HasForeignKey(sc => sc.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Delete shopping cart entries if user is deleted

            // Define relationship: ShoppingCart has one Stock, Stock can be in many ShoppingCarts
            modelBuilder.Entity<ShoppingCart>()
                .HasOne(sc => sc.Stock)
                .WithMany() // No navigation property in Stock model currently
                .HasForeignKey(sc => sc.StockId)
                .OnDelete(DeleteBehavior.Cascade); // Delete shopping cart entries if stock item is deleted
        }
    }
}
