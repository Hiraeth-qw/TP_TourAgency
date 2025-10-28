using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MicroserviceTour.Models
{
    public class TourContext : DbContext
    {
        public TourContext(DbContextOptions<TourContext> options) : base(options) { }
        public DbSet<Tour> Tour { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var converter = new ValueConverter<List<int>, string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions)null) ?? new List<int>());

            modelBuilder.Entity<Tour>()
                .Property(e => e.PartnerIds)
                .HasConversion(converter);
        }
    }
}
