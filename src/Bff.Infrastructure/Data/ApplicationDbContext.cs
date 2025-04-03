using Bff.Application.Common.Interfaces;
using Bff.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Bff.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantMember> TenantMembers => Set<TenantMember>();
    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
    public DbSet<BackupDataTenant> BackupDataTenants => Set<BackupDataTenant>();
    public DbSet<BackupDataTenantMember> BackupDataTenantMembers => Set<BackupDataTenantMember>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<Tenant>(tenant => {
            tenant.Property(e => e.IsWorkSpacePersonal)
                .IsRequired()
                .HasDefaultValue(false);
        });
    }
}