using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Application.Dtos;
using PerfSvc.Application.Object.Common;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Application.Object.Queries;

public class GetObjectByIdQuery : IRequest<ResultCustom<ObjectDtos>>
{
    public required Guid ObjectId { get; set; }
}

public class GetObjectByIdQueryHandler(
    IObjectRepository objectRepository,
    IObjectMemberRepository objectMemberRepository,
    IUnitRepository unitRepository,
    AuthorizationChecker authorizationChecker)
    : IRequestHandler<GetObjectByIdQuery, ResultCustom<ObjectDtos>>
{
    private readonly IObjectRepository _objectRepository = objectRepository;
    private readonly IObjectMemberRepository _objectMemberRepository = objectMemberRepository;
    private readonly IUnitRepository _unitRepository = unitRepository;
    private readonly AuthorizationChecker _authorizationChecker = authorizationChecker;


    public async Task<ResultCustom<ObjectDtos>> Handle(GetObjectByIdQuery rq, CancellationToken cancellationToken)
    {
        try
        {

            var obj = await _objectRepository.ReadObject(rq.ObjectId, cancellationToken);
            var memberList = await _objectMemberRepository.GetAllMemberFromObject(rq.ObjectId, "", cancellationToken);
            var memberObjects = memberList.Select(x => new { x.MemberId, x.MemberName }).ToList();
            var objectDto = new ObjectDtos()
            {
                Id = obj.Id,
                Title = obj.Title,
                CreatedDate = obj.CreatedDate,
                Description = obj.Description,
                DueDate = obj.DueDate,
                Priority = obj.Priority,
                UpdatedDate = obj.UpdatedDate,
                MemberIds = memberObjects.Select(x => x.MemberId).ToList(),
                MemberNames = memberObjects.Select(x => x.MemberName).ToList()
            };
            return new ResultCustom<ObjectDtos>()
            {
                Status = StatusCode.OK,
                Message = new[] { "Success" },
                Data = objectDto
            };
        }
        catch (Exception e)
        {
            return new ResultCustom<ObjectDtos>()
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { e.Message }
            };
        }
    }
}