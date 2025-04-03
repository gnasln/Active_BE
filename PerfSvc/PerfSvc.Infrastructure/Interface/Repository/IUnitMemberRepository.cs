using PerfSvc.Domain.Entities;

namespace PerfSvc.Infrastructure.Interface.Repository;

public interface IUnitMemberRepository 
{
    Task<Guid> AddUnitMember(UnitMember unitMember, CancellationToken cancellationToken);
    Task<List<UnitMember>> GetAllUnitMember(Guid unitId, string? memberName, CancellationToken cancellationToken);
    Task<UnitMember> UpdateUnitMember(UnitMember unitMember, CancellationToken cancellationToken);
    Task<Guid> DeleteUnitMember(Guid unitId, Guid memberId, CancellationToken cancellationToken);
    Task<UnitMember?> GetUnitMemberById(Guid unitId, Guid memberId, CancellationToken cancellationToken);
}