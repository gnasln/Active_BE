using AutoMapper;
using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Application.KeyResultMember.Commands;

public class AddMemberToKeyResultCommand : IRequest<ResultCustom<Guid>>
{
    public required Guid MemberId { get; set; }
    public required Guid KeyResultId { get; set; }
    public string? MemberName { get; set; }
    public string? MemberFullName { get; set; }
    
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<AddMemberToKeyResultCommand, Domain.Entities.KeyResultMember>();
        }
    }
}
public class AddMemberToKeyResultCommandHandler : IRequestHandler<AddMemberToKeyResultCommand, ResultCustom<Guid>>
{
    private readonly IKeyResultMemberRepository _keyResultMemberRepository;
    private readonly IMapper _mapper;

    public AddMemberToKeyResultCommandHandler(IKeyResultMemberRepository keyResultMemberRepository, IMapper mapper)
    {
        _keyResultMemberRepository = keyResultMemberRepository;
        _mapper = mapper;
    }

    public async Task<ResultCustom<Guid>> Handle(AddMemberToKeyResultCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var keyResultMember = _mapper.Map<Domain.Entities.KeyResultMember>(request);
            var result = await _keyResultMemberRepository.AddKeyResultMember(keyResultMember, cancellationToken);
            return new ResultCustom<Guid>()
            {
                Status = StatusCode.OK,
                Message = new []{"Member added to key result successfully"},
                Data = result
            };
        }
        catch (Exception e)
        {
            return new ResultCustom<Guid>()
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new []{e.Message}
            };
        }
    }
}