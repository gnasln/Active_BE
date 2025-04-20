using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Application.Object.Common;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Application.Object.Commands;

public class DeleteObjectCommand : IRequest<ResultCustom<string>>
{
    public required Guid ObjectTBId { get; set; }
}

public class DeleteObjectCommandHandle(
    IObjectRepository objectRepository,
    IUnitRepository unitRepository,
    AuthorizationChecker authorizationChecker) : IRequestHandler<DeleteObjectCommand, ResultCustom<string>>
{
    private readonly IObjectRepository _objectRepository = objectRepository;
    private readonly IUnitRepository _unitRepository = unitRepository;
    private readonly AuthorizationChecker _authorizationChecker = authorizationChecker;

    public async Task<ResultCustom<string>> Handle(DeleteObjectCommand rq, CancellationToken cancellationToken)
    {
        try
        {
            // Retrieve the object
            var checkObject = await _objectRepository.ReadObject(rq.ObjectTBId, cancellationToken);
            var unit = await _unitRepository.GetUnitById(checkObject.UnitId, cancellationToken);

            // Proceed to delete the object
            await _objectRepository.DeleteObject(rq.ObjectTBId, cancellationToken);
            return new ResultCustom<string>()
            {
                Status = StatusCode.OK,
                Message = new[] { "Success" },
                Data = "Delete Object Successfully"
            };
        }
        catch (Exception e)
        {
            return new ResultCustom<string>()
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { e.Message }
            };
        }
    }
}