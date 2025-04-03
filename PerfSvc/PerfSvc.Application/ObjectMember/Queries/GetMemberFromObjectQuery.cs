using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Application.ObjectMember.Queries;

public class GetMemberFromObjectQuery : IRequest<ResultCustom<List<Domain.Entities.ObjectMember>>>
{
    public required Guid ObjectTBId { get; set; }
}
public class GetMemberFromObjectQueryHandler : IRequestHandler<GetMemberFromObjectQuery, ResultCustom<List<Domain.Entities.ObjectMember>>>
{
    private readonly IObjectMemberRepository _objectMemberRepository;

    public GetMemberFromObjectQueryHandler(IObjectMemberRepository objectMemberRepository)
    {
        _objectMemberRepository = objectMemberRepository;
    }
    
    public async Task<ResultCustom<List<Domain.Entities.ObjectMember>>> Handle(GetMemberFromObjectQuery rq, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _objectMemberRepository.GetAllMemberFromObject(rq.ObjectTBId, "", cancellationToken);
            return new ResultCustom<List<Domain.Entities.ObjectMember>>()
            {
                Status = StatusCode.OK,
                Message = new []{"Success"},
                Data = result
            };
        }
        catch (Exception e)
        {
            return new ResultCustom<List<Domain.Entities.ObjectMember>>()
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { e.Message }
            };
        }
    }
}