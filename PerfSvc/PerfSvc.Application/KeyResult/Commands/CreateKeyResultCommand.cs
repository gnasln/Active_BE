using AutoMapper;
using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Application.Dtos;
using PerfSvc.Application.KeyResultMember.Commands;
using PerfSvc.Infrastructure.Interface.Repository;
using PrefSvc.Domain.Enums;

namespace PerfSvc.Application.KeyResult.Commands;

public class CreateKeyResultCommand : IRequest<ResultCustom<KeyResultDto>>
{
    public required string Title { get; set; }
    public string? Description { set; get; } = string.Empty;

    public DateTime? CreatedDate { set; get; } = DateTime.Now;
    public DateTime? DueDate { set; get; }

    public PriorityKeyResult? Priority { set; get; } = PriorityKeyResult.None;

    public Guid ObjectTBId { set; get; }

    public List<Guid>? MemberIds { set; get; }
    public List<string?>? MemberNames { set; get; }

    public class Mapper : Profile
    {
        public Mapper()
        {
            CreateMap<CreateKeyResultCommand, KeyResultDto>();
        }
    }
}

public class CreateKeyResultCommandHandler : IRequestHandler<CreateKeyResultCommand, ResultCustom<KeyResultDto>>
{
    private readonly IKeyResultRepository _keyResultRepository;
    private readonly IMapper _mapper;
    private readonly ISender _sender;

    public CreateKeyResultCommandHandler(IKeyResultRepository keyResultRepository, IMapper mapper, ISender sender)
    {
        _keyResultRepository = keyResultRepository;
        _mapper = mapper;
        _sender = sender;
    }

    public async Task<ResultCustom<KeyResultDto>> Handle(CreateKeyResultCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var keyResult = new Domain.Entities.KeyResult()
            {
                Title = request.Title,
                Description = request.Description,
                CreatedDate = request.CreatedDate,
                DueDate = request.DueDate,
                Priority = request.Priority,
                ObjectTBId = request.ObjectTBId
            };
            var keyResultId = await _keyResultRepository.CreateKeyResult(keyResult, cancellationToken);

            //add members to key result
            if (request.MemberIds != null && request.MemberIds.Count > 0)
            {
                var index = 0;
                foreach (var memberId in request.MemberIds)
                {
                    AddMemberToKeyResultCommand addMember = new()
                    {
                        MemberId = memberId,
                        MemberName = request.MemberNames?[index],
                        KeyResultId = keyResultId,
                        MemberFullName = ""
                    };
                    await _sender.Send(addMember, cancellationToken);
                    ++index;
                }
            }
            var result = _mapper.Map<KeyResultDto>(request);
            result.Id = keyResultId;

            return new ResultCustom<KeyResultDto>()
            {
                Status = StatusCode.OK,
                Message = new[] { "Key result created successfully" },
                Data = result
            };
        }
        catch (Exception e)
        {
            return new ResultCustom<KeyResultDto>()
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { e.Message }
            };
        }
    }
}