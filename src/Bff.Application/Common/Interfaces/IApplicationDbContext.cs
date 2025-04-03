
using Microsoft.EntityFrameworkCore;

namespace Bff.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Bff.Domain.Entities.Tenant> Tenants {get;}
    DbSet<Bff.Domain.Entities.TenantMember> TenantMembers {get;}
    DbSet<Bff.Domain.Entities.BackupDataTenant> BackupDataTenants {get;}
    DbSet<Bff.Domain.Entities.BackupDataTenantMember> BackupDataTenantMembers {get;}
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}