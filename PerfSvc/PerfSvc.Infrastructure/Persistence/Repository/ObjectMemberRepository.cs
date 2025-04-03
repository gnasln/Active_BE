using Microsoft.EntityFrameworkCore;
using PerfSvc.Domain.Entities;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Infrastructure.Persistence.Repository;

public class ObjectMemberRepository(IPerfDbContext context) : IObjectMemberRepository
{
    private readonly IPerfDbContext _context = context;
    public async Task<Guid> AddMemberToObject(ObjectMember objectMember, CancellationToken cancellationToken)
    {
        await _context.ObjectMembers.AddAsync(objectMember, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return objectMember.Id;
    }

    public async Task RemoveMemberFromObject(Guid memberObjectId, CancellationToken cancellationToken)
    {
        var result = await _context.ObjectMembers.FindAsync(memberObjectId, cancellationToken);
        if (result != null)
        {
            _context.ObjectMembers.Remove(result);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<List<ObjectMember>> GetAllMemberFromObject(Guid objectId, string? memberName, CancellationToken cancellationToken)
    {
        var result = await _context.ObjectMembers
            .Where(x => x.ObjectTBId == objectId && x.MemberName.Contains(memberName))
            .ToListAsync(cancellationToken);
        return result;
    }

    public async Task<ObjectMember?> GetMemberFromObject(Guid objectId, Guid memberId, CancellationToken cancellationToken)
    {
        var result = await _context.ObjectMembers
            .Where(x => x.ObjectTBId == objectId && x.MemberId == memberId)
            .FirstOrDefaultAsync(cancellationToken);
        return result;
    }
}