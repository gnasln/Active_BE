using Microsoft.EntityFrameworkCore;
using PerfSvc.Domain.Entities;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Infrastructure.Persistence.Repository;

public class KeyResultMemberRepository(IPerfDbContext context) : IKeyResultMemberRepository
{
    private readonly IPerfDbContext _context = context;
    public async Task<Guid> AddKeyResultMember(KeyResultMember keyResultMember, CancellationToken cancellationToken)
    {
        await _context.KeyResultMembers.AddAsync(keyResultMember, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return keyResultMember.Id;
    }

    public async Task<KeyResultMember?> GetKeyResultMember(Guid keyResultMemberId, CancellationToken cancellationToken)
    {
        var result = await _context.KeyResultMembers.FindAsync(keyResultMemberId, cancellationToken);
        return result;
    }

    public async Task<List<KeyResultMember>?> GetAllKeyResultMember(Guid keyResultId, CancellationToken cancellationToken)
    {
        var result = await _context.KeyResultMembers.Where(x => x.KeyResultId == keyResultId).ToListAsync(cancellationToken);
        return result;
    }

    public async Task DeleteKeyResultMember(Guid keyResultMemberId, CancellationToken cancellationToken)
    {
        var keyResultMember = await _context.KeyResultMembers.FindAsync(keyResultMemberId, cancellationToken);
        if (keyResultMember == null)
        {
            throw new Exception("KeyResultMember not found");
        }
        _context.KeyResultMembers.Remove(keyResultMember);
        await _context.SaveChangesAsync(cancellationToken);
    }
}