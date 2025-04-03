using Bff.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bff.Application.Tenants.Common;

/*
    cái này check người dùng đã có wsp cá nhân hay chưa 
    - nếu có => return true
    - chưa có => flase
*/

public record CheckExistWorkspacePersonal : IRequest<bool>
{
    public Guid UserId { set; get; }
}

public class CheckExistWorkspacePersonalHandle(IApplicationDbContext db) : IRequestHandler<CheckExistWorkspacePersonal, bool>
{
    private readonly IApplicationDbContext _db = db;

    public async Task<bool> Handle(CheckExistWorkspacePersonal qr, CancellationToken cancellationToken)
    {
        // check this user already exist workspacepesonal 
        var wsp = await _db.Tenants
            .Where(t => t.Owner == qr.UserId && t.IsWorkSpacePersonal == true)
            .FirstOrDefaultAsync(cancellationToken);

        if (wsp is null) return false;

        return true;
    }
}