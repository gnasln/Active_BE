using AutoMapper;
using Bff.Application.Common.Interfaces;
using Bff.Application.Dtos.Tenants;
using Bff.Application.Tenants.Queries;
using Bff.Domain.Entities;
using MediatR;
using NetHelper.Common.Models;

namespace Bff.Application.Tenants.Commands;

public record TenantCreateCommand : IRequest<ResultCustom<TenantRespone>>
{
    public required string Name { set; get; } = string.Empty;
    public string? Description { set; get; } = String.Empty;
    
    public DateTime CreatedDate {set; get;} = DateTime.Now;
    
    public List<Guid>? memberIds { set; get; }
    public List<string>? memberNames { set; get; }

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<TenantCreateCommand, Tenant>();
        }
    }

}

public class TenantCreateCommandHandler(IApplicationDbContext db, IUser user, IMapper mapper, IMediator mediator) : IRequestHandler<TenantCreateCommand, ResultCustom<TenantRespone>>
{
    private readonly IApplicationDbContext _db = db;
    private readonly IUser _user = user;
    private readonly IMapper _mapper = mapper;
    private readonly IMediator _mediator = mediator;
    public async Task<ResultCustom<TenantRespone>> Handle(TenantCreateCommand rq, CancellationToken cancellationToken)
    {
        try
        {
            var entity = _mapper.Map<TenantCreateCommand, Tenant>(rq);
            if (!string.IsNullOrEmpty(_user.Id))
            {
                entity.Owner = Guid.Parse(_user.Id);

            }
            if (!string.IsNullOrEmpty(_user.UserName))
            {
                entity.OwnerName = _user.UserName;
            }

            if (!string.IsNullOrEmpty(rq.Description))
            {
                entity.Description = rq.Description;
            }
            await _db.Tenants.AddAsync(entity, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);

            GetTenantByIdQuery Condition = new() { Id = entity.Id };
            var NewlyCreatedTenant = await _mediator.Send(Condition, cancellationToken);
            //them owner vao tenant
            TenantMember addOwner = new()
            {
                UserId = entity.Owner,
                UserName = entity.OwnerName,
                TenantId = entity.Id,
                UserFullName = ""
            };
            
            await _db.TenantMembers.AddAsync(addOwner, cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
            
            // thêm người dùng vào tổ chức
            
            if (rq.memberIds != null && rq.memberIds.Count > 0)
            {
                var index = 0;
                foreach (var memberId in rq.memberIds)
                {
                    AddMemberToTenantCommand addMember = new()
                    {
                        UserId = memberId,
                        UserName = rq.memberNames?[index],
                        TenantId = entity.Id
                    };
                    await _mediator.Send(addMember, cancellationToken);
                    ++index;
                }
            }

            return new ResultCustom<TenantRespone>
            {
                Status = StatusCode.CREATED,
                Message = new[] { $"Created Tenant successfully" },
                Data = new TenantRespone()
                {
                    Id = NewlyCreatedTenant.Data.Id,
                    Owner = NewlyCreatedTenant.Data.Owner,
                    OwnerName = NewlyCreatedTenant.Data.OwnerName,
                    Name = NewlyCreatedTenant.Data.Name,
                    Description = NewlyCreatedTenant.Data.Description,
                    CreatedDate = NewlyCreatedTenant.Data.CreatedDate,
                    memberIds = rq.memberIds?.ToList(),
                    memberNames = rq.memberNames?.ToList()
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