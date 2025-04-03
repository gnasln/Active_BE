using Bff.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using PerfSvc.Infrastructure;

namespace PerfSvc.Application.Unit.common;

public class CheckManagerOfUnit : IRequest<bool>
{
    public required Guid UnitId { get; set; }
}

public class CheckManagerOfUnithandle(IPerfDbContext db, IUser user) : IRequestHandler<CheckManagerOfUnit, bool>
{
    private readonly IPerfDbContext _db = db;
    private readonly IUser _user = user;
    public async Task<bool> Handle(CheckManagerOfUnit rq, CancellationToken cancellationToken)
    {   
        var check = await _db.Units
                .Where(u => _user.Id != null && u.ManagerId == Guid.Parse(_user.Id) && u.Id == rq.UnitId)
                .FirstOrDefaultAsync(cancellationToken)
            ;

        if(check is not null) return true;

        return false;
    }
}