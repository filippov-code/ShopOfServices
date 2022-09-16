﻿using Microsoft.EntityFrameworkCore;
using System;
using ShopOfServices.Models;
using System.Reflection;
using ShopOfServices.Models.Configurations;

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
            var assembly = Assembly.GetExecutingAssembly();
            modelBuilder.ApplyConfigurationsFromAssembly(assembly);
        }
    }
}