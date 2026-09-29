namespace HeapProject
{
    using HeapProject.Identity;
    using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore;

    public class ApplicationDBContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // explicitly enforce unique emails in the database index, even though Identity already enforces this at the application level (this is needed due to a race condition)

            builder.Entity<ApplicationUser>().HasIndex(u => u.NormalizedEmail).IsUnique();
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
