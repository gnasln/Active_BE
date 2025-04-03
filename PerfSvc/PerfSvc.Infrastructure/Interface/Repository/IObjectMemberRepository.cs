using PerfSvc.Domain.Entities;

namespace PerfSvc.Infrastructure.Interface.Repository;

public interface IObjectMemberRepository
{
    public Task<Guid> AddMemberToObject(ObjectMember objectMember, CancellationToken cancellationToken);
    public Task RemoveMemberFromObject(Guid memberObjectId, CancellationToken cancellationToken);
    public Task<List<ObjectMember>> GetAllMemberFromObject(Guid objectId, string? memberName, CancellationToken cancellationToken);
    public Task<ObjectMember?> GetMemberFromObject(Guid objectId, Guid memberId, CancellationToken cancellationToken);
    
}