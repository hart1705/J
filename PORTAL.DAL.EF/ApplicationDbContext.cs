using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using PORTAL.DAL.EF.Helper;
using PORTAL.DAL.EF.Interfaces;
using PORTAL.DAL.EF.Models;
using PORTAL.DAL.EF.Models.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PORTAL.DAL.EF
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IHttpContextAccessor httpContextAccessor = null)
            : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public DbSet<Annotation> Annotation { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<ApplicationRole> ApplicationRole { get; set; }
        public DbSet<ApplicationAction> ApplicationAction { get; set; }
        public DbSet<ApplicationPermission> ApplicationPermission { get; set; }
        public DbSet<ApplicationAction_ApplicationPermission> ApplicationAction_ApplicationPermission { get; set; }
        public DbSet<ApplicationRole_ApplicationPermission> ApplicationRole_ApplicationPermission { get; set; }
        public DbSet<ShortMessageService> ShortMessageService { get; set; }
        public DbSet<GSMModem> GSMModem { get; set; }
        public DbSet<ApplicationPermissionView> ApplicationPermissionView { get; set; }
        public DbSet<RoleView> RoleView { get; set; }
        public DbSet<ReferralCode> ReferralCode { get; set; }
        public DbSet<Bayanihan> Bayanihan { get; set; }
        public DbSet<Income> Income { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "User");
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });

            builder.Entity<ApplicationAction_ApplicationPermission>()
               .HasOne(pt => pt.ApplicationPermission)
               .WithMany(t => t.ApplicationAction_ApplicationPermissions)
               .HasForeignKey(pt => pt.ApplicationPermissionId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationAction_ApplicationPermission>()
                .HasOne(pt => pt.ApplicationAction)
                .WithMany(t => t.ApplicationAction_ApplicationPermissions)
                .HasForeignKey(pt => pt.ApplicationActionId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationRole_ApplicationPermission>()
                .HasKey(t => new { t.RoleId, t.ApplicationPermissionId });

            builder.Entity<ApplicationRole_ApplicationPermission>()
               .HasOne(pt => pt.ApplicationRole)
               .WithMany(t => t.ApplicationRole_ApplicationPermissions)
               .HasForeignKey(pt => pt.RoleId);

            builder.Entity<ApplicationRole_ApplicationPermission>()
                .HasOne(pt => pt.ApplicationPermission)
                .WithMany(t => t.ApplicationRole_ApplicationPermissions)
                .HasForeignKey(pt => pt.ApplicationPermissionId);

            builder.Entity<ApplicationUser>()
                .HasOne(pt => pt.Income)
                .WithOne(i => i.User)
                .HasForeignKey<Income>(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasOne(pt => pt.Bayanihan)
                .WithOne(i => i.User)
                .HasForeignKey<Bayanihan>(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public override int SaveChanges()
        {
            AuditEntities();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            AuditEntities();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void AuditEntities()
        {
            string userName = string.Empty;
            string userId = string.Empty;
            if (_httpContextAccessor != null && _httpContextAccessor.HttpContext != null &&
                _httpContextAccessor.HttpContext.User != null)
            {
                var identity = _httpContextAccessor.HttpContext.User.Identity;
                if (identity != null && identity.IsAuthenticated)
                {
                    userName = identity.Name;
                    var user = Users.Where(u => u.UserName == userName).FirstOrDefault();
                    if (user != null)
                    {
                        userId = user.Id;
                    }
                }
            }

            DateTime now = DateTime.Now;
            foreach (EntityEntry<IAuditable> entry in ChangeTracker.Entries<IAuditable>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Property("CreatedOn").CurrentValue = now;
                    entry.Property("ModifiedOn").CurrentValue = now;
                    entry.Property("Status").CurrentValue = Enums.Status.Active;
                    if (!string.IsNullOrWhiteSpace(userId))
                    {
                        entry.Property("ModifiedBy").CurrentValue = userId;
                        entry.Property("CreatedBy").CurrentValue = userId;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Property("ModifiedOn").CurrentValue = now;
                    if (!string.IsNullOrWhiteSpace(userId))
                    {
                        entry.Property("ModifiedBy").CurrentValue = userId;
                    }
                    entry.Property("CreatedBy").IsModified = false;
                    entry.Property("CreatedOn").IsModified = false;
                    if (entry.Property("Status").CurrentValue == null)
                    {
                        entry.Property("Status").IsModified = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Called when executing migration
    /// Define to remove exception "Add an implementation of ‘IDesignTimeDbContextFactory’ to the project"
    /// https://codingblast.com/entityframework-core-add-implementation-idesigntimedbcontextfactory-multiple-dbcontexts/
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = OptionsBuilder();
            return new ApplicationDbContext(builder.Options);
        }

        public DbContextOptionsBuilder<ApplicationDbContext> OptionsBuilder()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseSqlServer(connectionString);
            return builder;
        }
    }
}
