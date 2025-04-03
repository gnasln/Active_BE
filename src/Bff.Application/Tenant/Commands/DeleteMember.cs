using Bff.Application.Common.Interfaces;
using Bff.Application.Tenants.Common;
using Bff.Application.Tenants.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetHelper.Common.Models;
using Npgsql;

namespace Bff.Application.Tenants.Commands;

public record DeleteMemberCommand : IRequest<ResultCustom<string>>
{
    public Guid UserId { set; get; }
    public Guid TenantId { set; get; }

}

public class DeleteMemberCommandHandle(IApplicationDbContext db, IMediator mediator, IConfiguration configuration) : IRequestHandler<DeleteMemberCommand, ResultCustom<string>>
{
    private readonly IMediator _mediator = mediator;
    private readonly IApplicationDbContext _db = db;
    private readonly IConfiguration _configuration = configuration;

    public async Task<ResultCustom<string>> Handle(DeleteMemberCommand rq, CancellationToken cancellationToken)
    {
        try
        {
            // check tenant 
            GetTenantByIdQuery tenantQr = new() { Id = rq.TenantId };
            var checkTenant = await _mediator.Send(tenantQr);
            if (checkTenant is null) return new ResultCustom<string>
            {
                Status = StatusCode.NOTFOUND,
                Message = new[] { "Tenant Id doesn't exist" }
            };

            // check owner 
            CheckOwner owner = new() { Id = checkTenant.Data.Owner };
            var checkOwner = await _mediator.Send(owner, cancellationToken);
            if (!checkOwner) return new ResultCustom<string>
            {
                Status = StatusCode.FORBIDDEN,
                Message = new[] { "Forbidden !" }
            };

            // check member exist to Tenant 
            SearchMemberToTenant member = new() { UserId = rq.UserId, TenantId = rq.TenantId };
            var checkMemberExist = await _mediator.Send(member, cancellationToken);
            if (!checkMemberExist) return new ResultCustom<string>
            {
                Status = StatusCode.NOTFOUND,
                Message = new[] { "This user does't the Tenant !" }
            };

            // delete 
            var MemberDel = await _db.TenantMembers
                .Where(tm => tm.UserId == rq.UserId && tm.TenantId == rq.TenantId)
                .FirstOrDefaultAsync(cancellationToken);

            if (MemberDel != null)
            {
                _db.TenantMembers.Remove(MemberDel);
                await _db.SaveChangesAsync(cancellationToken);
                
                // update tenant name in user
                string connectionString = _configuration.GetConnectionString("BffCnt");
                await using (var conn = new NpgsqlConnection(connectionString))
                {
                    await conn.OpenAsync(cancellationToken);
                    await using (var cmd = new NpgsqlCommand("UPDATE \"AspNetUsers\" SET \"Tenant\" = @Tenant WHERE \"Id\" = @UserId", conn))
                    {
                        cmd.Parameters.AddWithValue("Tenant", "");
                        cmd.Parameters.AddWithValue("UserId", rq.UserId.ToString());
                        await cmd.ExecuteNonQueryAsync(cancellationToken);
                    }

                }

                return new ResultCustom<string>
                {
                    Status = StatusCode.OK,
                    Message = new [] {$"remove the member id: {rq.UserId} from the tenant id : {rq.TenantId} "}
                };
                
            }

            return new ResultCustom<string>
            {
                Status = StatusCode.BADREQUEST,
                Message = new [] {"Error, please try again in a few minutes"}
            };

        }
        catch (Exception ex)
        {
            return new ResultCustom<string>
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { $"ERROR :: {ex}" }
            };
        }
    }
}