using AutoMapper;
using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Infrastructure.Interface.Repository;

namespace PerfSvc.Application.ObjectMember.Commands;

public record AddMemberToObjectCommand : IRequest<ResultCustom<Guid>>
{
    public required Guid ObjectTBId { get; set; }
    public required Guid MemberId { get; set; }
    public string? MemberName { get; set; }
    public string? MemberFullName { get; set; }
    
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<AddMemberToObjectCommand, Domain.Entities.ObjectMember>();
        }
    }
}
public class AddMemberToObjectCommandHandler : IRequestHandler<AddMemberToObjectCommand, ResultCustom<Guid>>
{
    private readonly IMapper _mapper;
    private readonly ISender _sender;
    private readonly IObjectMemberRepository _objectMemberRepository;
    private readonly IObjectRepository _objectRepository;

    public AddMemberToObjectCommandHandler(IMapper mapper, ISender sender, IObjectMemberRepository objectMemberRepository, IObjectRepository objectRepository)
    {
        _mapper = mapper;
        _sender = sender;
        _objectMemberRepository = objectMemberRepository;
        _objectRepository = objectRepository;
    }
    
    public async Task<ResultCustom<Guid>> Handle(AddMemberToObjectCommand rq, CancellationToken cancellationToken)
    {
        try
        {
            // check object
            var checkObject = await _objectRepository.ReadObject(rq.ObjectTBId, cancellationToken);
            if (checkObject == null)
                return new ResultCustom<Guid>()
                {
                    Status = StatusCode.NOTFOUND,
                    Message = new[] { "Object doesn't exist" }
                };

            // check member in object
            var checkMemberInObject = await _objectMemberRepository.GetMemberFromObject(rq.ObjectTBId, rq.MemberId, cancellationToken);
            if (checkMemberInObject != null)
                return new ResultCustom<Guid>()
                {
                    Status = StatusCode.BADREQUEST,
                    Message = new[] { "Member already in object" }
                };

            // add member to object
            var member = _mapper.Map<Domain.Entities.ObjectMember>(rq);
            var result = await _objectMemberRepository.AddMemberToObject(member, cancellationToken);
            return new ResultCustom<Guid>()
            {
                Status = StatusCode.CREATED,
                Message = new[] { "Member added to object successfully" },
                Data = result
            };
        }
        catch (Exception e)
        {
            return new ResultCustom<Guid>()
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { e.Message }
            };
        }
    }
}