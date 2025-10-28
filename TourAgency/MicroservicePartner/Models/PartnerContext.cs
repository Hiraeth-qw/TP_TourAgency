using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
namespace MicroservicePartner.Models
{
    public class PartnerContext : DbContext
    {
        public PartnerContext(DbContextOptions<PartnerContext> options) : base(options) { }
        public DbSet<Partner> Partners { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Partner>()
                .Property(p => p.PartnerType)
                .HasConversion<string>();

            modelBuilder.Entity<partnerHotel>();
            modelBuilder.Entity<partnerOperator>();
            modelBuilder.Entity<partnerTransport>();
        }
    }
}
