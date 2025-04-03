using Bff.Application.Common.Interfaces;
using MediatR;

namespace Bff.Application.Tenants.Common;

public record CheckTenantExist : IRequest<bool>
{
    public Guid TenantId {set; get;}

}

public class CheckTenantExistHandle(IApplicationDbContext db) : IRequestHandler<CheckTenantExist, bool>
{
    private readonly IApplicationDbContext _db = db;
    public async Task<bool> Handle(CheckTenantExist rq, CancellationToken cancellationToken)
    {
        var tenant = await _db.Tenants.FindAsync([rq.TenantId], cancellationToken);
        if(tenant is null) return false;

        return true;
    }
}