using AutoMapper;
using MediatR;
using NetHelper.Common.Models;
using PrefSvc.Domain.Enums;
using PerfSvc.Infrastructure.Interface.Repository;
using Bff.Application.Tenants.Common;
using PerfSvc.Application.Dtos;
using PerfSvc.Application.UnitMember.Commands;

namespace PerfSvc.Application.Unit.Commands
{
    public record CreateUnitCommand : IRequest<ResultCustom<UnitSimple>>
    {
        public required string Name { set; get; }
        public string? Description { set; get; } = string.Empty;
        public Guid ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public PriorityUnit? Priority { get; set; } = PriorityUnit.None;
        public DateTime? DueDate { get; set; }
        public DateTime? CreatedDate { set; get; } = DateTime.Now;
        public Guid? ParentUnitId { get; set; }
        public required Guid TenantId { set; get; }

        public List<Guid> memberIds { get; set; }
        public List<string> memberNames { get; set; }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<CreateUnitCommand, UnitSimple>();
            }
        }

        public class CreateUnitCommandHandler : IRequestHandler<CreateUnitCommand, ResultCustom<UnitSimple>>
        {
            private readonly IUnitRepository _unitRepository;
            private readonly IMapper _mapper;
            private readonly ISender _sender;

            public CreateUnitCommandHandler(IUnitRepository unitRepository, IMapper mapper, ISender sender)
            {
                _unitRepository = unitRepository;
                _mapper = mapper;
                _sender = sender;
            }

            public async Task<ResultCustom<UnitSimple>> Handle(CreateUnitCommand request, CancellationToken cancellationToken)
            {
                try
                {
                    // check tenant exist 
                    CheckTenantExist t = new() { TenantId = request.TenantId };
                    var checkTenantExist = await _sender.Send(t, cancellationToken);
                    if (!checkTenantExist)
                        return new ResultCustom<UnitSimple>
                        {
                            Status = StatusCode.NOTFOUND,
                            Message = new[] { "This tenant is not exist, can't create unit" }
                        };

                    // check owner of tenant 
                    //CheckOwnerOfTenant o = new() { TenantId = request.TenantId };
                    //var checkIsOwner = await _sender.Send(o, cancellationToken);
                    //if (!checkIsOwner)
                    //    return new ResultCustom<UnitSimple>
                    //    {
                    //        Status = StatusCode.FORBIDDEN,
                    //        Message = new[] { "Forbidden" }
                    //    };

                    var unit = new Domain.Entities.Unit
                    {
                        Name = request.Name,
                        Description = request.Description,
                        ManagerId = request.ManagerId,
                        ManagerName = request.ManagerName,
                        Priority = request.Priority,
                        DueDate = request.DueDate,
                        CreatedDate = request.CreatedDate,
                        ParentUnitId = request.ParentUnitId,
                        TenantId = request.TenantId
                    };
                    var result = await _unitRepository.CreateUnit(unit, cancellationToken);

                    if (request.memberIds != null && request.memberIds.Count > 0)
                    {
                        var index = 0;
                        foreach (var memberId in request.memberIds)
                        {
                            AddMemberToUnitCommand addMember = new()
                            {
                                MemberId = memberId,
                                MemberName = request.memberNames[index],
                                UnitId = result.Id
                            };
                            await _sender.Send(addMember, cancellationToken);
                            ++index;
                        }
                    }

                    return new ResultCustom<UnitSimple>
                    {
                        Status = StatusCode.CREATED,
                        Message = new[] { "Unit created successfully" },
                        Data = _mapper.Map<UnitSimple>(result)
                    };
                }
                catch (Exception ex)
                {
                    return new ResultCustom<UnitSimple>
                    {
                        Status = StatusCode.INTERNALSERVERERROR,
                        Message = new[] { $"ERROR :: {ex}" }
                    };
                }
            }
        }
    }
}