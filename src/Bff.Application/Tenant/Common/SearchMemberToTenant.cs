using Bff.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bff.Application.Tenants.Common;
public record SearchMemberToTenant : IRequest<bool>
{
    public Guid UserId { set; get; }
    public Guid TenantId { set; get; }

}

public class SearchMemberToTenantHandle(IApplicationDbContext db) : IRequestHandler<SearchMemberToTenant, bool>
{
    private readonly IApplicationDbContext _db = db;
    public async Task<bool> Handle(SearchMemberToTenant query, CancellationToken cancellationToken)
    {
        var entity = await _db.TenantMembers
            .Where(tm => tm.UserId == query.UserId && tm.TenantId == query.TenantId)
            .Select(tm => tm.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (entity != Guid.Empty) return true;
        return false;
    }
}