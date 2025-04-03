using AutoMapper;
using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Application.KeyResultMember.Queries;

public class GetMemberFromKeyResultQuery : IRequest<ResultCustom<List<Domain.Entities.KeyResultMember>>>
{
    public required Guid KeyResultId { get; set; }
}
public class GetMemberFromKeyResultQueryHandler : IRequestHandler<GetMemberFromKeyResultQuery, ResultCustom<List<Domain.Entities.KeyResultMember>>>
{
    private readonly IKeyResultMemberRepository _keyResultMemberRepository;

    public GetMemberFromKeyResultQueryHandler(IKeyResultMemberRepository keyResultMemberRepository, IMapper mapper)
    {
        _keyResultMemberRepository = keyResultMemberRepository;
    }

    public async Task<ResultCustom<List<Domain.Entities.KeyResultMember>>> Handle(GetMemberFromKeyResultQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _keyResultMemberRepository.GetAllKeyResultMember(request.KeyResultId, cancellationToken);
            return new ResultCustom<List<Domain.Entities.KeyResultMember>>()
            {
                Status = StatusCode.OK,
                Message = new []{"Members retrieved successfully"},
                Data = result
            };
        }
        catch (Exception e)
        {
            return new ResultCustom<List<Domain.Entities.KeyResultMember>>()
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new []{e.Message}
            };
        }
    }
}