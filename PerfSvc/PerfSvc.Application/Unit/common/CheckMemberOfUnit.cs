using Bff.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PerfSvc.Infrastructure;

namespace PerfSvc.Application.Unit.common;

public class CheckMemberOfUnit : IRequest<bool>
{
    public required Guid UnitId { get; set; }
}

public class CheckMemberOfUnitHandle(IPerfDbContext db, IUser user) : IRequestHandler<CheckMemberOfUnit, bool>
{
    private readonly IPerfDbContext _db = db;
    private readonly IUser _user = user;
    public async Task<bool> Handle(CheckMemberOfUnit rq, CancellationToken cancellationToken)
    {   
        var check = await _db.UnitMembers
                .Where(u => _user.Id != null && u.MemberId == Guid.Parse(_user.Id) && u.UnitId == rq.UnitId)
                .FirstOrDefaultAsync(cancellationToken)
            ;

        if(check is not null) return true;

        return false;
    }
}