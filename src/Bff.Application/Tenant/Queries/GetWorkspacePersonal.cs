using Bff.Application.Common.Interfaces;
using Bff.Application.Dtos.Tenants;
using Bff.Application.Tenants.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Models;

namespace Bff.Application.Tenants.Queries;

public record GetWorkspacePersonalQuery : IRequest<ResultCustom<TenantRespone>> { }

public class GetWorkspacePersonalQueryHandle(IApplicationDbContext db, IMediator mediator, IUser user) : IRequestHandler<GetWorkspacePersonalQuery, ResultCustom<TenantRespone>>
{
    private readonly IApplicationDbContext _db = db;
    private readonly IMediator _mediator = mediator;
    private readonly IUser _user = user;

    public async Task<ResultCustom<TenantRespone>> Handle(GetWorkspacePersonalQuery query, CancellationToken cancellationToken)
    {
        try
        {
            // check exist if not exist then create new wsp 
            CheckExistWorkspacePersonalAndCreate wsp = new() { Owner = Guid.Parse(_user.Id), OwnerName = _user.UserName };
            await _mediator.Send(wsp);

            // get wsp 
            var workspace = await _db.Tenants
                .Where(t => t.Owner == Guid.Parse(_user.Id) && t.IsWorkSpacePersonal == true)
                .FirstOrDefaultAsync(cancellationToken);

            return new ResultCustom<TenantRespone>
            {
                Status = StatusCode.OK,
                Message = new[] { "get workspace personal successfully !" },
                Data = new TenantRespone()
                {
                    Id = workspace.Id,
                    Name = workspace.Name,
                    Owner = workspace.Owner,
                    OwnerName = workspace.OwnerName,
                    Description = workspace.Description,
                    CreatedDate = workspace.CreatedDate,
                    IsWorkSpacePersonal = workspace.IsWorkSpacePersonal
                }
            };

        }
        catch (Exception ex)
        {
            return new ResultCustom<TenantRespone>
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { $"ERROR :: {ex}" }
            };
        }
    }
}