using System.Security.Authentication;
using Bff.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bff.Application.Tenants.Common;

public record CheckOwnerOfTenant : IRequest<bool>
{
    public Guid TenantId {set; get;}

}

public class CheckOwnerOfTenanthandle(IApplicationDbContext db, IUser user) : IRequestHandler<CheckOwnerOfTenant, bool>
{
    private readonly IApplicationDbContext _db = db;
    private readonly IUser _user = user;
    public async Task<bool> Handle(CheckOwnerOfTenant rq, CancellationToken cancellationToken)
    {   
        var check = await _db.Tenants
        .Where(t => _user.Id != null && t.Owner == Guid.Parse(_user.Id) && t.Id == rq.TenantId)
        .FirstOrDefaultAsync(cancellationToken)
        ;

        if(check is not null) return true;

        return false;
    }
}