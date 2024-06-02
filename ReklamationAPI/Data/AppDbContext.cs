using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ReklamationAPI.Models;
using ReklamationAPI.Validation;

namespace ReklamationAPI.data
{

    /// <summary>
    /// Context class for Database access.
    /// </summary>
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }

        public AppDbContext() : base() { }

        public AppDbContext(DbContextOptions options) : base(options) { }

        /// <summary>
        /// Child implementation of OnConfiguring.
        /// </summary>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlite("Data Source=database/app.db");
            }
            base.OnConfiguring(optionsBuilder);
        }

        /// <summary>
        /// Child implementation of OnModelCreating.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Write name of status to DB rather than enum-generated ID.
            modelBuilder
                .Entity<Complaint>()
                .Property(d => d.Status)
                .HasConversion(new EnumToStringConverter<ComplaintStatus>());
        }

    }
}