using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Application.Object.Common;
using PerfSvc.Domain.Dtos;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Application.Object.Queries;

public class GetAllObjectFromUnitQuery : IRequest<ResultCustom<PaginatedList<ObjectDto>>>
{
    public required Guid UnitId { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class GetAllObjectFromUnitQueryHandler : IRequestHandler<GetAllObjectFromUnitQuery, ResultCustom<PaginatedList<ObjectDto>>>
{
    private readonly IObjectRepository _objectRepository;
    private readonly AuthorizationChecker _authorizationChecker;
    private readonly IUnitRepository _unitRepository;

    public GetAllObjectFromUnitQueryHandler(IObjectRepository objectRepository, ISender sender,
        IUnitRepository unitRepository, AuthorizationChecker authorizationChecker)
    {
        _objectRepository = objectRepository;
        _authorizationChecker = authorizationChecker;
        _unitRepository = unitRepository;
    }

    public async Task<ResultCustom<PaginatedList<ObjectDto>>> Handle(GetAllObjectFromUnitQuery request,
        CancellationToken cancellationToken)
    {
        try
        {
            //get all object from unit
            var result = await _objectRepository.GetAllObject(request.UnitId, request.PageNumber, request.PageSize, cancellationToken);
            return new ResultCustom<PaginatedList<ObjectDto>>()
            {
                Status = StatusCode.OK,
                Message = new[] { "Success" },
                Data = result
            };
        }
        catch (Exception e)
        {
            return new ResultCustom<PaginatedList<ObjectDto>>()
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { e.Message }
            };
        }
    }
}