using Microsoft.EntityFrameworkCore;
using RestaurantWebsiteApplication.Models;

namespace RestaurantWebsiteApplication.Data
{
    public class RestaurantDbContext : DbContext
    {
        public RestaurantDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Customer> Customerdata { get; set; }
        public DbSet<Admin> Admindata { get; set; }
        public DbSet<Booking> Bookingdata { get; set; }
        public DbSet<CheckIn> CheckIns { get; set; }
        public DbSet<CheckOut> CheckOuts { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
            .HasMany(c => c.Bookingdata)  // A customer has many bookings
            .WithOne(b => b.Customer)  // Each booking has one customer
            .HasForeignKey(b => b.UserId);  // The foreign key is UserId

            base.OnModelCreating(modelBuilder);
        }
    }
}
