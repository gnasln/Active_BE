using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Application.KeyResult.Commands;

public class DeleteKeyResultCommand : IRequest<ResultCustom<string>>
{
    public required Guid KeyResultId { get; set; }
}

public class DeleteKeyResultCommandHandle(IKeyResultRepository keyResultRepository) : IRequestHandler<DeleteKeyResultCommand, ResultCustom<string>>
{
    private readonly IKeyResultRepository _keyResultRepository = keyResultRepository;
    public async Task<ResultCustom<string>> Handle(DeleteKeyResultCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _keyResultRepository.DeleteKeyResult(request.KeyResultId, cancellationToken);
            return new ResultCustom<string>()
            {
                Status = StatusCode.OK,
                Message = new[] { "Delete key result successfully" }
            };
        }
        catch (Exception e)
        {
            return new ResultCustom<string>()
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { "Error when delete key result: " + e.Message }
            };
        }
    }
}