using PerfSvc.Domain.Entities;

namespace PerfSvc.Infrastructure.Interface.Repository;

public interface IKeyResultMemberRepository
{
    public Task<Guid> AddKeyResultMember(KeyResultMember keyResultMember, CancellationToken cancellationToken);
    public Task<KeyResultMember?> GetKeyResultMember(Guid keyResultMemberId, CancellationToken cancellationToken);
    public Task<List<KeyResultMember>?> GetAllKeyResultMember(Guid keyResultId, CancellationToken cancellationToken);
    public Task DeleteKeyResultMember(Guid keyResultMemberId, CancellationToken cancellationToken);
}