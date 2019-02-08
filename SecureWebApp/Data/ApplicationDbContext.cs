using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SALUSUAV_Demo.Models;
using SALUSUAV_Demo.Models.FlightData;
using SALUSUAV_Demo.Models.UAVData;

namespace SALUSUAV_DEMO.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // ReSharper disable once RedundantOverriddenMember
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<FlightData> FlightData { get; set; }

        public DbSet<FlightDetails> FlightDetails { get; set; }

        public DbSet<UavData> UavData { get; set; }

    }
}
