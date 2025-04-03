using Microsoft.EntityFrameworkCore;
using PerfSvc.Domain.Entities;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Infrastructure.Persistence.Repository;

public class UnitMemberRepository : IUnitMemberRepository
{
    private readonly PerfDbContext _context;
    
    public UnitMemberRepository(PerfDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> AddUnitMember(UnitMember unitMember, CancellationToken cancellationToken)
    {
        await _context.UnitMembers.AddAsync(unitMember, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return unitMember.Id;   
    }

    public async Task<List<UnitMember>> GetAllUnitMember(Guid unitId, string? memberName, CancellationToken cancellationToken)
    {
        var query = _context.UnitMembers.Where(x => x.UnitId == unitId && (string.IsNullOrEmpty(memberName) || x.MemberName.Contains(memberName.ToLower())));
        var members = await query.ToListAsync(cancellationToken);
        return members;
    }

    public async Task<UnitMember> UpdateUnitMember(UnitMember unitMember, CancellationToken cancellationToken)
    {
        var result = await _context.UnitMembers.FindAsync(unitMember.Id, cancellationToken);
        result!.UnitId = unitMember.UnitId;
        result.MemberName = unitMember.MemberName ?? result.MemberName;
        result.MemberFullName = unitMember.MemberFullName ?? result.MemberFullName;
        _context.UnitMembers.Update(result);
        return result;
    }

    public async Task<Guid> DeleteUnitMember(Guid unitId, Guid memberId, CancellationToken cancellationToken)
    {
        var result = await _context.UnitMembers.FindAsync(memberId, cancellationToken);
        _context.UnitMembers.Remove(result!);
        return result!.Id;
    }

    public async Task<UnitMember?> GetUnitMemberById(Guid unitId, Guid memberId, CancellationToken cancellationToken)
    {
        var result = await _context.UnitMembers.FindAsync(memberId, cancellationToken);
        return result;
    }
}