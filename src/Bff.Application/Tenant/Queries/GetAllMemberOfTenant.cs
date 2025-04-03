
using Bff.Application.Common.Interfaces;
using Bff.Application.Tenants.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetHelper.Common.Models;

namespace Bff.Application.Tenants.Queries;
public class MemberList
{
    public Guid UserId { set; get; }
    public string? UserName { set; get; }

}

public record GetAllMemberOfTenantQuery : IRequest<ResultCustom<List<MemberList>>>
{
    // phải biết nó là tổ chức nào 
    public Guid Id { set; get; }
}

public class GetAllMemberOfTenantQueryHandle(IApplicationDbContext db, IMediator mediator) : IRequestHandler<GetAllMemberOfTenantQuery, ResultCustom<List<MemberList>>>
{
    private readonly IApplicationDbContext _db = db;
    private readonly IMediator _mediator = mediator;

    public async Task<ResultCustom<List<MemberList>>> Handle(GetAllMemberOfTenantQuery query, CancellationToken cancellationToken)
    {
        try
        {
            // get tenant and check tenant exit
            GetTenantByIdQuery tenantQr = new() { Id = query.Id };
            var checkTenant = await _mediator.Send(tenantQr, cancellationToken);
            if (checkTenant is null) return new ResultCustom<List<MemberList>>
            {
                Status = StatusCode.NOTFOUND,
                Message = new[] { "Id Tenant doesn't exist !" }
            };

            // if tenant exist , check is owner 
            CheckOwner owner = new() { Id = checkTenant.Data.Owner };
            var checkOwner = await _mediator.Send(owner, cancellationToken);

            if (!checkOwner) return new ResultCustom<List<MemberList>>
            {
                Status = StatusCode.FORBIDDEN,
                Message = new[] { "Forbiden" }
            };

            // get all member 
            var ListAllMember = await _db.TenantMembers
                .Where(tm => tm.TenantId == query.Id)
                .Select(tm => new MemberList()
                {
                    UserId = tm.UserId,
                    UserName = tm.UserName
                })
                .ToListAsync(cancellationToken);


            return new ResultCustom<List<MemberList>>
            {
                Status = StatusCode.OK,
                Message = new[] { "Get successfully" },
                Data = ListAllMember
            };
        }
        catch (Exception ex)
        {
            return new ResultCustom<List<MemberList>>
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { $"ERROR :: {ex}" }
            };
        }
    }
}