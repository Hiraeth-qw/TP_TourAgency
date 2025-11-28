using Microsoft.EntityFrameworkCore;

namespace MicroserviceBooking.Models
{
    public class BookingContext : DbContext
    {
        public BookingContext(DbContextOptions<BookingContext> options) : base(options) { }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<SingleConfirmation> SingleConfirmations { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>()
                .Property(p => p.Status)
                .HasConversion<string>();
        }
    }
}