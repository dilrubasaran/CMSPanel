using identity_signup.Areas.Instructor.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace identity_singup.Models
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Education> Education { get; set; }
        public DbSet<PermissionRequest> PermissionRequests { get; set; }
        public DbSet<LoginAudit> LoginAudits { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PermissionRequest>()
                .Property(p => p.RequestDate)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Education>()
                .Property(e => e.EduPrice)
                .HasPrecision(9, 2);  // 9 toplam basamak, 2 ondalık basamak

            modelBuilder.Entity<LoginAudit>()
                .Property(l => l.LoginTime)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<MenuItem>()
                .HasOne(m => m.Parent)
                .WithMany(m => m.Children)
                .HasForeignKey(m => m.ParentId);
        }

    }
}
