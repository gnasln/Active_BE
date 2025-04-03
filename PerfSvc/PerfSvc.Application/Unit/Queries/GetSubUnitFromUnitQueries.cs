using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Application.Unit.Queries;

public record GetSubUnitFromUnitQueries : IRequest<ResultCustom<List<Domain.Entities.Unit>>>
{
    public required Guid ParentId { get; set; }
}

public class
    GetSubUnitFromUnitQueriesHandle : IRequestHandler<GetSubUnitFromUnitQueries,
    ResultCustom<List<Domain.Entities.Unit>>>
{
    private readonly IUnitRepository _repo;

    public GetSubUnitFromUnitQueriesHandle(IUnitRepository repo)
    {
        _repo = repo;
    }

    public async Task<ResultCustom<List<Domain.Entities.Unit>>> Handle(GetSubUnitFromUnitQueries query,
        CancellationToken cancellationToken)
    {
        try
        {
            var data = await _repo.GetUnitByParentId(query.ParentId, cancellationToken);

            return new ResultCustom<List<Domain.Entities.Unit>>
            {
                Status = StatusCode.OK,
                Message = new[] { "Get all sub unit from unit successfully" },
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