using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace WebSecurity.Models
{
    public class TestDbContext : DbContext
    {
        public TestDbContext():base("DefaultConnection")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<TestTable1> TestTable1 { get; set; }
        public DbSet<SessionManager> SessionManager { get; set; }
        public DbSet<ConfigManager> ConfigManager { get; set; }
    }
}