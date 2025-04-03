using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Application.ObjectMember.Commands;

public class DeleteMemberFromObjectCommand : IRequest<ResultCustom<string>>
{
    public required Guid MemberId { get; set; }
}
public class DeleteMemberFromObjectCommandHandle(IObjectMemberRepository objectMemberRepository, ISender sender) : IRequestHandler<DeleteMemberFromObjectCommand, ResultCustom<string>>
{
    private readonly IObjectMemberRepository _objectMemberRepository = objectMemberRepository;
    private readonly ISender _sender = sender;

    public async Task<ResultCustom<string>> Handle(DeleteMemberFromObjectCommand rq, CancellationToken cancellationToken)
    {
        try
        {
            await _objectMemberRepository.RemoveMemberFromObject(rq.MemberId, cancellationToken);
            return new ResultCustom<string>()
            {
                Status = StatusCode.OK,
                Message = new []{"Success"},
                Data = "Delete Object Member Successfully}"
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