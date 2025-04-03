using AutoMapper.Execution;
using MediatR;
using NetHelper.Common.Models;
using Bff.Application.Common.Interfaces;
using Bff.Application.Tenants.Common;
using Bff.Application.Dtos.Tenants;
using Bff.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bff.Application.Tenants.Commands;

public record TenantUpdateCommand : IRequest<ResultCustom<TenantRespone>>
{
    public Guid Id { set; get; }
    public string Name { set; get; } = string.Empty;
    public Guid? Owner { set; get; }
    public string? OwnerName { set; get; } = string.Empty;
    public string? Description { set; get; } = string.Empty;
    public List<Guid>? memberIds { set; get; }
    public List<string?>? memberNames { set; get; }

}

public class TenantUpdateCommandHandle(IApplicationDbContext db, IMediator mediator) : IRequestHandler<TenantUpdateCommand, ResultCustom<TenantRespone>>
{
    private readonly IApplicationDbContext _db = db;
    private readonly IMediator _mediator = mediator;

    public async Task<ResultCustom<TenantRespone>> Handle(TenantUpdateCommand rq, CancellationToken cancellationToken)
    {
        try
        {
            // check tenant exist
            var entity = await _db.Tenants.FindAsync(rq.Id, cancellationToken);
            if (entity is null)
                return new ResultCustom<TenantRespone>
                {
                    Status = StatusCode.NOTFOUND,
                    Message = new[] { "This Tenant doesn't exist " }
                };

            // check if it is the owner of tenant
            CheckOwner check = new() { Id = entity.Owner };
            var checkOwner = await _mediator.Send(check, cancellationToken);

            if (!checkOwner) return new ResultCustom<TenantRespone>
            {
                Status = StatusCode.FORBIDDEN,
                Message = new[] { "Forbidden" }
            };
            
            if (rq.Owner is not null && rq.OwnerName is not null)
            {
                entity.Owner = rq.Owner.Value;
                entity.OwnerName = rq.OwnerName;
            }
            if(rq.Description is not null) entity.Description = rq.Description;

            if (rq.memberIds is not null && rq.memberNames is not null)
            {
                var memberList = await _db.TenantMembers
                    .Where(tm => tm.TenantId == rq.Id)
                    .ToListAsync(cancellationToken);

                foreach (var member in memberList)
                {
                    var deleteMemberCommand = new DeleteMemberCommand
                    {
                        UserId = member.UserId,
                        TenantId = rq.Id
                    };
                    await _mediator.Send(deleteMemberCommand, cancellationToken);
                }

                for (int i = 0; i < rq.memberIds.Count; i++)
                {
                    var addMemberCommand = new AddMemberToTenantCommand
                    {
                        UserId = rq.memberIds[i],
                        UserName = rq.memberNames[i],
                        TenantId = rq.Id
                    };
                    await _mediator.Send(addMemberCommand, cancellationToken);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);

            return new ResultCustom<TenantRespone>
            {
                Status = StatusCode.OK,
                Message = new[] { "Update Tenant successfully" },
                Data = new TenantRespone() {
                    Id = entity.Id,
                    Name = entity.Name,
                    Owner = entity.Owner,
                    OwnerName = entity.OwnerName,
                    Description =  entity.Description,
                    CreatedDate = entity.CreatedDate,
                    memberIds = rq.memberIds,
                    memberNames = rq.memberNames
                }
            };
        }
        catch (Exception ex)
        {
            return new ResultCustom<TenantRespone>
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { $"{ex}" }
            };
        }
    }
}
