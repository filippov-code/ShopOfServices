using Microsoft.EntityFrameworkCore;
using System;
using ShopOfServices.Models;
using System.Reflection;
using ShopOfServices.Models.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace ShopOfServices.Data
{
    public class SiteDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Service> Services { get; set; }

        public DbSet<Specialist> Specialists { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Page> Pages { get; set; }


        public SiteDbContext(DbContextOptions<SiteDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var assembly = Assembly.GetExecutingAssembly();
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }
    }
}
