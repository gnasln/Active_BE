using Bff.Application.Common.Interfaces;
using MediatR;
using NetHelper.Common.Models;
using AutoMapper;
using Bff.Application.Tenants.Common;
using Bff.Domain.Entities;
using Bff.Application.Dtos.Tenants;


namespace Bff.Application.Tenants.Commands;

public record CreateWorkSpaceCommand : IRequest<ResultCustom<TenantRespone>>
{
    public string Name { set; get; } = "WorkspacePersonal";
    public bool IsWorkSpacePersonal { set; get; } = true;
    public string? Description { set; get; } = string.Empty;

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<CreateWorkSpaceCommand, Tenant>();
        }
    }
}

public class CreateWorkSpaceCommandHandle(IApplicationDbContext db, IUser user, IMediator mediator, IMapper mapper) : IRequestHandler<CreateWorkSpaceCommand, ResultCustom<TenantRespone>>
{
    private readonly IApplicationDbContext _db = db;
    private readonly IUser _user = user;
    private readonly IMediator _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    public async Task<ResultCustom<TenantRespone>> Handle(CreateWorkSpaceCommand rq, CancellationToken cancellationToken)
    {
        try
        {
            // check this user already exist " workspace pesonal"
            CheckExistWorkspacePersonal wsp = new() { UserId = Guid.Parse(_user.Id) };
            var checkWsp = await _mediator.Send(wsp, cancellationToken);
            // if exist 
            if (checkWsp) return new ResultCustom<TenantRespone>
            {
                Status = StatusCode.CONFLICT,
                Message = new[] { "This User already exist workspace personal " }
            };

            // if not exist 
            var entity = _mapper.Map<CreateWorkSpaceCommand, Tenant>(rq);
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

            return new ResultCustom<TenantRespone>
            {
                Status = StatusCode.CREATED,
                Message = new[] { "Created workspace personal successfully" }
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