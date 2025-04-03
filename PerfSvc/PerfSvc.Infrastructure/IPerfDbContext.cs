using Microsoft.EntityFrameworkCore;

namespace PerfSvc.Infrastructure;

public interface IPerfDbContext
{
    DbSet<Domain.Entities.Unit> Units { get; }
    DbSet<Domain.Entities.UnitMember> UnitMembers { get; }
    DbSet<Domain.Entities.ObjectTB> ObjectTBs { get; }
    DbSet<Domain.Entities.KeyResult> KeyResults { get; }
    DbSet<Domain.Entities.KeyresultHistory> KeyresultHistories { get; }
    DbSet<Domain.Entities.ObjectHistory>  objectHistories { get; }
    DbSet<Domain.Entities.ObjectMember> ObjectMembers { get; }
    DbSet<Domain.Entities.KeyResultMember> KeyResultMembers { get; }

    Task<int> SaveChangeAsync(CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
