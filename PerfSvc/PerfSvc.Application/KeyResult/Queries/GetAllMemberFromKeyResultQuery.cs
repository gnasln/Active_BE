using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Application.KeyResult.Queries;

public class GetAllKeyResultQuery : IRequest<ResultCustom<PaginatedList<Domain.Entities.KeyResult>>>
{
    public required Guid ObjectId { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}

public class GetAllMemberFromKeyResultQueryHandle(IKeyResultRepository keyResultRepository) : IRequestHandler<GetAllKeyResultQuery,
    ResultCustom<PaginatedList<Domain.Entities.KeyResult>>>
{
    private readonly IKeyResultRepository _keyResultRepository = keyResultRepository;
    public async Task<ResultCustom<PaginatedList<Domain.Entities.KeyResult>>> Handle(GetAllKeyResultQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _keyResultRepository.GetAllKeyResult(request.ObjectId, request.PageNumber, request.PageSize, cancellationToken);
            return new ResultCustom<PaginatedList<Domain.Entities.KeyResult>>()
            {
                Status = StatusCode.OK,
                Message = new[] { "Getkey result successfully" },
                Data = result
            };
        }
        catch (Exception e)
        {
            return new ResultCustom<PaginatedList<Domain.Entities.KeyResult>>()
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { "Error when get all key result: " + e.Message }
            };
        }
    }
}