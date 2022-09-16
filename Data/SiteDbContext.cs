using Microsoft.EntityFrameworkCore;
using System;
using ShopOfServices.Models;
using System.Reflection;

namespace ShopOfServices.Data
{
    public class SiteDbContext : DbContext
    {
        public DbSet<Service> Services { get; set; }

        public SiteDbContext(DbContextOptions<SiteDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
