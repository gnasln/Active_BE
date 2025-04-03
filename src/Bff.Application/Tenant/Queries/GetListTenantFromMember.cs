using Bff.Application.Common.Interfaces;
using Bff.Application.Dtos.Tenants;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Models;

namespace Bff.Application.Tenants.Queries;

public record GetListTenantByAdminQuery : IRequest<ResultCustom<List<TenantRespone>>>
{

}


public class GetListTenantByAdminQueryHandle(IApplicationDbContext db, IMediator mediator) : IRequestHandler<GetListTenantByAdminQuery, ResultCustom<List<TenantRespone>>>
{
    private readonly IApplicationDbContext _db = db;
    private readonly IMediator _mediator = mediator;

    public async Task<ResultCustom<List<TenantRespone>>> Handle(GetListTenantByAdminQuery query, CancellationToken cancellationToken)
    {
        try
        {

            // get list tenant by admin 
            var ListTenant = await _db.Tenants.ToListAsync();
            var result = new List<TenantRespone>();
            foreach (var tenant in ListTenant)
            {
                //get memberids and membernames of each tenant
                var memberIds = new List<Guid>();
                var memberNames = new List<string?>();
                GetAllMemberOfTenantQuery getAllMember = new GetAllMemberOfTenantQuery { Id = tenant.Id };
                var res = await _mediator.Send(getAllMember);
                if (res.Status == StatusCode.OK)
                {
                    memberIds = res.Data!.Select(member => member.UserId).ToList();
                    memberNames = res.Data!.Select(member => member.UserName).ToList();
                }
                result.Add(new TenantRespone
                {
                    Id = tenant.Id,
                    Name = tenant.Name,
                    Description = tenant.Description,
                    CreatedDate = tenant.CreatedDate,
                    Owner = tenant.Owner,
                    OwnerName = tenant.OwnerName,
                    memberIds = memberIds,
                    memberNames = memberNames
                });
            }

            return new ResultCustom<List<TenantRespone>>
            {
                Status = StatusCode.OK,
                Message = new[] { "Get List tenant successfully" },
                Data = result
            };
        }
        catch (Exception ex)
        {

            return new ResultCustom<List<TenantRespone>>
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { $"ERROR :: {ex}" }
            };
        }
    }
}