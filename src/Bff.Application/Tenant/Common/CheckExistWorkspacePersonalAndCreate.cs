using Bff.Application.Common.Interfaces;
using MediatR;
using AutoMapper;
using Bff.Domain.Entities;

/*
Cái này nếu kiểm tra người dùng xem đã có workspace các nhân hay chưa 
- nếu như chưa có => tạo mới wsp cá nhân và trả về true
- nếu như có rồi => trả về false 
*/

namespace Bff.Application.Tenants.Common;

public record CheckExistWorkspacePersonalAndCreate : IRequest<bool>
{
    public Guid Owner { set; get; }

    public required string OwnerName { set; get; }


}

public class CheckExistWorkspacePersonalAndCreateHandle(IApplicationDbContext db, IMediator mediator, IMapper mapper) : IRequestHandler<CheckExistWorkspacePersonalAndCreate, bool>
{
    private readonly IApplicationDbContext _db = db;
    private readonly IMediator _mediator = mediator;
    private readonly IMapper _mapper = mapper;

    public async Task<bool> Handle(CheckExistWorkspacePersonalAndCreate rq, CancellationToken cancellationToken)
    {
        // check 
        CheckExistWorkspacePersonal wsp = new() { UserId = rq.Owner };
        var checkWsp = await _mediator.Send(wsp);

        // if exist 
        if (!checkWsp)
        {
            // create new wsp 
            var entity = new Tenant() {
                Name = "workspace personal",
                Owner = rq.Owner,
                OwnerName = rq.OwnerName,
                IsWorkSpacePersonal = true
            };
            await _db.Tenants.AddAsync(entity);
            await _db.SaveChangesAsync(cancellationToken);

            return true;
        }

        return false;


    }
}