using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UfaSmartCity.Models;

namespace UfaSmartCity.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<StationModel> Stations { get; set; }
        public DbSet<ModuleType> ModuleTypes { get; set; }
        public DbSet<Content> Contents { get; set; }
        public DbSet<InformationTable> InformationTables { get; set; }
    }
}
