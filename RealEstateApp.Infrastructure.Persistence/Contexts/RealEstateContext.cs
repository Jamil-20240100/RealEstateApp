using Microsoft.EntityFrameworkCore;
using RealEstateApp.Infrastructure.Identity.Entities;
using System.Reflection;

namespace RealEstateApp.Infrastructure.Persistence.Contexts
{
    public class RealEstateContext : DbContext
    {
        public RealEstateContext(DbContextOptions<RealEstateContext> options) : base(options) { }

        //
        // DB SETS
        //
        
        public DbSet<AppUser> Users { get; set; }

        //
        // ENTITY CONFIGURATIONS APPLICATION
        //

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}