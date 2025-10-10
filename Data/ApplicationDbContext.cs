using CarRentalSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CarRentalSystem.Models;

namespace CarRentalSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Car> Cars { get; set; }
        public DbSet<Rental> Rentals { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Car entity configuration
            builder.Entity<Car>(entity =>
            {
                entity.HasQueryFilter(e => !e.IsDeleted);
                entity.Property(e => e.PricePerDay).HasPrecision(18, 2);
            });

            // Rental entity configuration
            builder.Entity<Rental>(entity =>
            {
                entity.HasQueryFilter(e => !e.IsDeleted);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);

                entity.HasOne(r => r.Car)
                      .WithMany(c => c.Rentals)
                      .HasForeignKey(r => r.CarId);

                entity.HasOne(r => r.User)
                      .WithMany(u => u.Rentals)
                      .HasForeignKey(r => r.UserId);
            });
        }
    }
}
