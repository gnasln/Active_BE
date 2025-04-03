using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Infrastructure.Interface.Repository;
using Bff.Application.Tenants.Common;

namespace PerfSvc.Application.Unit.Queries;

public record GetUnitFromTenantQuery : IRequest<ResultCustom<List<Domain.Entities.Unit>>>
{
    public required Guid TenantId { set; get; }
}

public class GetUnitFromTenantQueryHandle(IUnitRepository repo, ISender sender) : IRequestHandler<GetUnitFromTenantQuery, ResultCustom<List<Domain.Entities.Unit>>>
{
    private readonly IUnitRepository _repo = repo;
    private readonly ISender _sender = sender;
    

    public async Task<ResultCustom<List<Domain.Entities.Unit>>> Handle(GetUnitFromTenantQuery query, CancellationToken cancellationToken)
    {

        try
        {

            var data = await _repo.GetAllUnitFromTenant(query.TenantId, cancellationToken);

            return new ResultCustom<List<Domain.Entities.Unit>>
            {
                Status = StatusCode.OK,
                Message = new[] { "Get all unit from tenant successfully" },
                Data = data
            };
        }
        catch (Exception ex)
        {
            return new ResultCustom<List<Domain.Entities.Unit>>
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { $"ERROR :: {ex}" }
            };
        }
    }
}