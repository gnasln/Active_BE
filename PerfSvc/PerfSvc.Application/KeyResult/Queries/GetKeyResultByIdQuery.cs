using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Application.Dtos;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Application.KeyResult.Queries;

public class GetKeyResultByIdQuery : IRequest<ResultCustom<KeyResultDto>>
{
    public required Guid KeyResultId { get; set; }
}
public class GetKeyResultByIdQueryHandler : IRequestHandler<GetKeyResultByIdQuery, ResultCustom<KeyResultDto>>
{
    private readonly IKeyResultRepository _keyResultRepository;
    private readonly IKeyResultMemberRepository _keyResultMemberRepository;

    public GetKeyResultByIdQueryHandler(IKeyResultRepository keyResultRepository, IKeyResultMemberRepository keyResultMemberRepository)
    {
        _keyResultRepository = keyResultRepository;
        _keyResultMemberRepository = keyResultMemberRepository;
    }

    public async Task<ResultCustom<KeyResultDto>> Handle(GetKeyResultByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var keyResult = await _keyResultRepository.ReadKeyResult(request.KeyResultId, cancellationToken);
            var listMemberKeyResults =
                await _keyResultMemberRepository.GetAllKeyResultMember(request.KeyResultId, cancellationToken);
            var memberKeyResults = listMemberKeyResults?.Select(x => new { x.MemberId, x.MemberName }).ToList();
            var result = new KeyResultDto()
            {
                Id = keyResult.Id,
                Title = keyResult.Title,
                CreatedDate = keyResult.CreatedDate,
                Description = keyResult.Description,
                DueDate = keyResult.DueDate,
                Priority = keyResult.Priority,
                ObjectTBId = keyResult.ObjectTBId,
                MemberIds = memberKeyResults?.Select(x => x.MemberId).ToList(),
                MemberNames = memberKeyResults?.Select(x => x.MemberName).ToList()
                
            };
            return new ResultCustom<KeyResultDto>()
            {
                Status = StatusCode.OK,
                Message = new[] { "Get key result by id successfully" },
                Data = result
            };
        }
        catch (Exception e)
        {
            return new ResultCustom<KeyResultDto>()
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { "Error when get key result by id: " + e.Message }
            };
        }
    }
}