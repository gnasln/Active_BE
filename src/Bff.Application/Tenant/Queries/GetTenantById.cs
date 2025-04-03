using MediatR;
using Bff.Application.Common.Interfaces;
using Bff.Application.Dtos.Tenants;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Models;

namespace Bff.Application.Tenants.Queries;

public record GetTenantByIdQuery : IRequest<ResultCustom<TenantRespone>>
{
    public Guid Id { set; get; }
}

public class GetTenantByIdQueryHandle(IApplicationDbContext db, IMediator mediator) : IRequestHandler<GetTenantByIdQuery, ResultCustom<TenantRespone>>
{
    private readonly IApplicationDbContext _db = db;
    private readonly IMediator _mediator = mediator;

    public async Task<ResultCustom<TenantRespone>> Handle(GetTenantByIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var tenant = await _db.Tenants.FindAsync(query.Id, cancellationToken);
            var memberList = await _db.TenantMembers
                .Where(tm => tm.TenantId == query.Id)
                .Select(tm => new MemberList()
                {
                    UserId = tm.UserId,
                    UserName = tm.UserName
                })
                .ToListAsync(cancellationToken);
            var data = new TenantRespone()
            {
                Id = tenant!.Id,
                Name = tenant.Name,
                Owner = tenant.Owner,
                OwnerName = tenant.OwnerName,
                Description = tenant.Description,
                CreatedDate = tenant.CreatedDate,
                IsWorkSpacePersonal = tenant.IsWorkSpacePersonal,
                memberIds = memberList.Select(m => m.UserId).ToList(),
                memberNames = memberList.Select(m => m.UserName).ToList()
            };
            return new ResultCustom<TenantRespone>()
            {
                Status = StatusCode.OK,
                Message = new []{"Get tenant success"},
                Data = data
            };
        }
        catch (Exception e)
        {
            return new ResultCustom<TenantRespone>()
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { e.Message }
            };
        }
    }
}