using Bff.Application.Common.Interfaces;
using Bff.Application.Tenants.Common;
using Bff.Application.Tenants.Queries;
using MediatR;
using NetHelper.Common.Models;
using AutoMapper;
using Bff.Domain.Entities;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Bff.Application.Tenants.Commands;

public record AddMemberToTenantCommand : IRequest<ResultCustom<string>>
{
    public Guid UserId { set; get; }
    public string? UserName { set; get; } = string.Empty;
    public Guid TenantId { set; get; }
    public string? UserFullName { set; get; } = string.Empty;

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<AddMemberToTenantCommand, TenantMember>();
        }
    }
}

public class AddMemberToTenantCommandHandle(IApplicationDbContext db, IMediator mediator, IMapper mapper, IConfiguration configuration) : IRequestHandler<AddMemberToTenantCommand, ResultCustom<string>>
{
    private readonly IApplicationDbContext _db = db;
    private readonly IMediator _mediator = mediator;
    private readonly IMapper _mapper = mapper;
    private readonly IConfiguration _configuration = configuration;

    public async Task<ResultCustom<string>> Handle(AddMemberToTenantCommand rq, CancellationToken cancellationToken)
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

            // if property isWorkspacePersonal is true return false
            if(checkTenant.Data.IsWorkSpacePersonal) return new ResultCustom<string>
            {
                Status = StatusCode.FORBIDDEN,
                Message = new [] {"Personal workspaces are not allowed to add members"}
            };

            // check owner
            CheckOwner owner = new() { Id = checkTenant.Data.Owner };
            var checkOwner = await _mediator.Send(owner, cancellationToken);
            if (!checkOwner) return new ResultCustom<string>
            {
                Status = StatusCode.FORBIDDEN,
                Message = new[] { "Forbidden !" }
            };

            // if add member is owner
            if (checkTenant.Data.Owner == rq.UserId) return new ResultCustom<string>
            {
                Status = StatusCode.CONFLICT,
                Message = new[] { "This person is the owner, this person cannot be added" }
            };

            // if already a member
            SearchMemberToTenant member = new() { UserId = rq.UserId, TenantId = rq.TenantId };
            var checkMemberExist = await _mediator.Send(member, cancellationToken);

            if (checkMemberExist) return new ResultCustom<string>
            {
                Status = StatusCode.CONFLICT,
                Message = new[] { "this user already a member of tenant!" }
            };

            // update tenant name in user
            string connectionString = _configuration.GetConnectionString("BffCnt");
            await using (var conn = new NpgsqlConnection(connectionString))
            {
                await conn.OpenAsync(cancellationToken);
                await using (var cmd = new NpgsqlCommand("UPDATE \"AspNetUsers\" SET \"Tenant\" = @Tenant WHERE \"Id\" = @UserId", conn))
                {
                    cmd.Parameters.AddWithValue("Tenant", checkTenant.Data.Name.ToString());
                    cmd.Parameters.AddWithValue("UserId", rq.UserId.ToString());
                    await cmd.ExecuteNonQueryAsync(cancellationToken);
                }

            }
            
            var entity = _mapper.Map<AddMemberToTenantCommand, TenantMember>(rq);
            // add member to tenant
            await _db.TenantMembers.AddAsync(entity, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            return new ResultCustom<string>
            {
                Status = StatusCode.CREATED,
                Message = new[] { $"Add Member id: {entity.UserId} to tenant {entity.TenantId} successfully! " },
                Data = entity.Id.ToString()
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