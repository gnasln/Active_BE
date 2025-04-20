using AutoMapper;
using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Application.Dtos;
using PerfSvc.Application.Object.Common;
using PerfSvc.Application.ObjectMember.Commands;
using PerfSvc.Infrastructure.Interface.Repository;
using PrefSvc.Domain.Enums;

namespace PerfSvc.Application.Object.Commands;

public record CreateObjectCommand : IRequest<ResultCustom<ObjectDtos>>
{
    public required string Title { get; set; }
    public string? Description { set; get; } = string.Empty;
    public PriorityObject? Priority { get; set; } = PriorityObject.None;
    public DateTime? CreatedDate { get; set; } = DateTime.Now;
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public Guid UnitId { get; set; }
    public List<Guid>? MemberIds { get; set; }
    public List<string>? MemberNames { get; set; }
    
    public class ObjectMappingProfile : Profile
    {
        public ObjectMappingProfile()
        {
            CreateMap<CreateObjectCommand, ObjectDtos>();
        }
    }
}

public class CreateObjectCommandHandler(
    IObjectRepository objectRepository,
    AuthorizationChecker authorizationChecker,
    IUnitRepository unitRepository,
    ISender sender, IMapper mapper) : IRequestHandler<CreateObjectCommand, ResultCustom<ObjectDtos>>
{
    private readonly IObjectRepository _objectRepository = objectRepository;
    private readonly AuthorizationChecker _authorizationChecker = authorizationChecker;
    private readonly IUnitRepository _unitRepository = unitRepository;
    private readonly ISender _sender = sender;
    private readonly IMapper _mapper = mapper;

    public async Task<ResultCustom<ObjectDtos>> Handle(CreateObjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var unit = await _unitRepository.GetUnitById(request.UnitId, cancellationToken);
            //create object
            var obj = new ObjectTB()
            {
                Title = request.Title,
                Description = request.Description,
                Priority = request.Priority,
                CreatedDate = request.CreatedDate,
                UpdatedDate = request.UpdatedDate,
                DueDate = request.DueDate,
                UnitId = request.UnitId
            };
            var objId = await _objectRepository.CreateObject(obj, cancellationToken);

            //add member to object
            if (request.MemberIds != null && request.MemberIds.Count > 0)
            {
                var index = 0;
                foreach (var memberId in request.MemberIds)
                {
                    AddMemberToObjectCommand addMember = new()
                    {
                        MemberId = memberId,
                        MemberName = request.MemberNames?[index],
                        ObjectTBId = objId
                    };
                    await _sender.Send(addMember, cancellationToken);
                    ++index;
                }
            }
            
            //return result
            var result =_mapper.Map<ObjectDtos>(request);
            result.Id = objId;
            result.UnitName = unit.Name;

            return new ResultCustom<ObjectDtos>()
            {
                Status = StatusCode.CREATED,
                Message = new[] { "Object created successfully" },
                Data = result
            };
        }
        catch (Exception ex)
        {
            return new ResultCustom<ObjectDtos>()
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { ex.Message }
            };
        }
    }
}