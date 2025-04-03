
using Bff.Application.Common.Interfaces;
using Bff.Application.Tenants.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Models;
using Bff.Application.Dtos.Tenants;

namespace Bff.Application.Tenants.Queries;

public record GetListTenantFromOwnerQuery : IRequest<ResultCustom<List<TenantRespone>>>
{

}

public class GetListTenantFromOwnerQueryHandle(IApplicationDbContext db, IUser user, IMediator mediator) : IRequestHandler<GetListTenantFromOwnerQuery, ResultCustom<List<TenantRespone>>>
{
    private readonly IApplicationDbContext _db = db;
    private readonly IUser _user = user;

    private readonly IMediator _mediator = mediator;

    public async Task<ResultCustom<List<TenantRespone>>> Handle(GetListTenantFromOwnerQuery query, CancellationToken cancellationToken)
    {
        try
        {
            // check user 
            CheckOwner user = new() { Id = Guid.Parse(_user.Id) };
            var check = await _mediator.Send(user, cancellationToken);
            if (!check) return new ResultCustom<List<TenantRespone>>
            {
                Status = StatusCode.FORBIDDEN,
                Message = new[] { "Forbidden " }
            };

            var data = await _db.Tenants
            .Where(t => t.Owner == Guid.Parse(_user.Id) && t.IsWorkSpacePersonal == false)
            .Select(t => new TenantRespone()
            {
                Id = t.Id,
                Name = t.Name,
                Owner = t.Owner,
                OwnerName = t.OwnerName,
                Description = t.Description,
                CreatedDate = t.CreatedDate
            })
            .ToListAsync(cancellationToken);
            
            // get list tenant that user is member 
            
            // get list teannt from member 
            var ListTenant = await _db.TenantMembers
                .Where(tm => tm.UserId == Guid.Parse(_user.Id))
                .Select(tm => new ListTenantFromMember()
                {
                    TenantId = tm.TenantId
                })
                .ToListAsync(cancellationToken);
            
            var data1 = new List<TenantRespone>();
            foreach (var tenant in ListTenant)
            {
                var tenantDetails = await _db.Tenants
                    .Where(t => t.Id == tenant.TenantId)
                    .Select(t => new TenantRespone
                    {
                        Id = t.Id,
                        Name = t.Name,
                        Owner = t.Owner,
                        OwnerName = t.OwnerName,
                        Description = t.Description,
                        CreatedDate = t.CreatedDate
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                if (tenantDetails != null)
                {
                    data1.Add(tenantDetails);
                }
            }

            // Merge data and data1, ensuring no duplicates
            var mergedData = data.Concat(data1)
                .GroupBy(t => t.Id)
                .Select(g => g.First())
                .ToList();

            return new ResultCustom<List<TenantRespone>>
            {
                Status = StatusCode.OK,
                Message = new[] { "Get list tenant successfully!" },
                Data = mergedData
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