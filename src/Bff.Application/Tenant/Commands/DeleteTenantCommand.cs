using Bff.Application.Common.Interfaces;
using Bff.Application.Tenants.Common;
using MediatR;
using NetHelper.Common.Models;

namespace Bff.Application.Tenants.Commands;
public class DeleteTenantCommand : IRequest<ResultCustom<string>>
{
    public required Guid TenantId { set; get; }
}

public class DeleteTenantCommandHandler : IRequestHandler<DeleteTenantCommand, ResultCustom<string>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMediator _mediator;

    public DeleteTenantCommandHandler(IApplicationDbContext context, IMediator mediator)
    {
        _context = context;
        _mediator = mediator;
    }

    public async Task<ResultCustom<string>> Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var entity = await _context.Tenants.FindAsync(request.TenantId, cancellationToken);
            if (entity == null)
            {
                return new ResultCustom<string>
                {
                    Status = StatusCode.NOTFOUND,
                    Message = new[] { "Tenant not found" }
                };
            }

            _context.Tenants.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return new ResultCustom<string>
            {
                Status = StatusCode.OK,
                Message = new[] { "Tenant deleted successfully" }
            };
        }
        catch (Exception ex)
        {
            return new ResultCustom<string>
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { ex.Message }
            };
        }
    }
}
