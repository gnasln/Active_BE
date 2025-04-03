using Microsoft.EntityFrameworkCore;
using PerfSvc.Domain.Entities;
using System.Reflection;

namespace PerfSvc.Infrastructure;

public class PerfDbContext(DbContextOptions<PerfDbContext> options) : DbContext(options), IPerfDbContext
{
    
    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<UnitMember> UnitMembers => Set<UnitMember>();
    public DbSet<ObjectTB> ObjectTBs => Set<ObjectTB>();
    public DbSet<KeyResult> KeyResults => Set<KeyResult>();
    public DbSet<KeyresultHistory> KeyresultHistories => Set<KeyresultHistory>();
    public DbSet<ObjectHistory> objectHistories => Set<ObjectHistory>();
    public DbSet<ObjectMember> ObjectMembers => Set<ObjectMember>();
    public DbSet<KeyResultMember> KeyResultMembers => Set<KeyResultMember>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    Task<int> IPerfDbContext.SaveChangeAsync(CancellationToken cancellationToken)
    {
        return base.SaveChangesAsync();
    }

    Task IPerfDbContext.SaveChangesAsync(CancellationToken cancellationToken)
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}
