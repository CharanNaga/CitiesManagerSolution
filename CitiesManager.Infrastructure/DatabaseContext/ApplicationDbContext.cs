using CitiesManager.Core.Entities;
using CitiesManager.Core.Identities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CitiesManager.Infrastructure.DatabaseContext
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
    {
        public ApplicationDbContext()
        {
        }
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public virtual DbSet<City> Cities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Adding Entity with a Table Name
            modelBuilder.Entity<City>().ToTable("Cities");

            //Adding Seed Data
            modelBuilder.Entity<City>().HasData(
                new City()
                {
                    CityID = Guid.Parse("99737F36-84FC-412B-BA0F-F592BF0D2257"),
                    CityName = "London"
                });

            modelBuilder.Entity<City>().HasData(
               new City()
               {
                   CityID = Guid.Parse("1A81EC9E-DD06-4C90-96D3-D3EF2B9217A6"),
                   CityName = "Milan"
               });

        }
    }
}
