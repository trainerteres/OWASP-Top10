using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace WebSecurity.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);

            //ApplicationUser currentUser = new ApplicationUser();

            //using (var db = new TestDbContext())
            //{
            //    var userId = userIdentity.GetUserId();
            //    var sessionObj = await db.SessionManager.FirstOrDefaultAsync(x => x.UserId == userId);

            //    if(sessionObj!=null)
            //    {
            //        userIdentity.AddClaim(new Claim(CookieExtractor.Claim_LastIssuedOn,sessionObj.LastCookieReleaseTime.ToString()));
            //    }
            //}

            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {       
        public DbSet<ConfigManager> ConfigManager { get; set; }

        public DbSet<SessionManager> SessionManager { get; set; }

        public DbSet<TestTable1> TestTable1 { get; set; }
       
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        static ApplicationDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);
           
            modelBuilder.Entity<ApplicationUser>().HasKey(m => m.Id);

            modelBuilder.Entity<IdentityUserLogin>().HasKey(m => new { m.LoginProvider, m.ProviderKey, m.UserId });

            modelBuilder.Entity<IdentityUserClaim>().HasKey(m => m.Id);

            modelBuilder.Entity<IdentityRole>().HasKey(m => m.Id);

            modelBuilder.Entity<IdentityUserRole>().HasKey(m => new { m.UserId, m.RoleId });
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
}