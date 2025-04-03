using MediatR;
using NetHelper.Common.Models;
using PrefSvc.Domain.Enums;
using PerfSvc.Infrastructure.Interface.Repository;
using AutoMapper;
using Bff.Application.Tenants.Common;
using PerfSvc.Application.Dtos;
using PerfSvc.Application.UnitMember.Queries;

namespace PerfSvc.Application.Unit.Commands
{
    public record UpdateUnitCommand : IRequest<ResultCustom<UnitSimple>>
    {
        public required Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public Guid? ParentUnitId { get; set; }
        public PriorityUnit? Priority { get; set; }
        public Guid? ManagerId { get; set; }
        public string? ManagerName { get; set; }
        public required Guid TenantId { get; set; }
        public List<Guid>? memberUnitIds { get; set; }
        public List<string>? memberUnitNames { get; set; }

        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<UpdateUnitCommand, UnitSimple>();
            }
        }
    }

    public class UpdateUnitCommandHandler : IRequestHandler<UpdateUnitCommand, ResultCustom<UnitSimple>>
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;
        private readonly ISender _sender;
        private readonly IUnitMemberRepository _unitMemberRepository;

        public UpdateUnitCommandHandler(IUnitRepository unitRepository, IMapper mapper, ISender sender, IUnitMemberRepository unitMemberRepository)
        {
            _unitRepository = unitRepository;
            _mapper = mapper;
            _sender = sender;
            _unitMemberRepository = unitMemberRepository;
        }

        public async Task<ResultCustom<UnitSimple>> Handle(UpdateUnitCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if tenant exists
                CheckTenantExist t = new() { TenantId = request.TenantId };
                var checkTenantExist = await _sender.Send(t, cancellationToken);
                if (!checkTenantExist)
                    return new ResultCustom<UnitSimple>
                    {
                        Status = StatusCode.NOTFOUND,
                        Message = new[] { "This tenant does not exist, can't create unit" }
                    };

                // Check if requester is the owner of the tenant
                CheckOwnerOfTenant o = new() { TenantId = request.TenantId };
                var checkIsOwner = await _sender.Send(o, cancellationToken);
                if (!checkIsOwner)
                    return new ResultCustom<UnitSimple>
                    {
                        Status = StatusCode.FORBIDDEN,
                        Message = new[] { "Forbidden" }
                    };

                // Retrieve the unit by its ID
                var unit = await _unitRepository.GetUnitById(request.Id, cancellationToken);
                if (unit == null)
                    return new ResultCustom<UnitSimple>
                    {
                        Status = StatusCode.NOTFOUND,
                        Message = new[] { "Unit not found" }
                    };

                // Update unit properties
                if (request.Name != null) unit.Name = request.Name;
                if (request.Description != null) unit.Description = request.Description;
                if (request.DueDate != null) unit.DueDate = request.DueDate;
                if (request.ParentUnitId != null) unit.ParentUnitId = request.ParentUnitId;
                if (request.Priority != null) unit.Priority = request.Priority;
                if (request.ManagerId != null) unit.ManagerId = request.ManagerId;
                if (request.ManagerName != null) unit.ManagerName = request.ManagerName;

                // Update members
                if (request.memberUnitIds != null && request.memberUnitNames != null)
                {
                    var existingMembers = (await _sender.Send(new GetListUnitMemberQuery() { UnitId = request.Id }, cancellationToken)).Data;
                    foreach (var member in existingMembers)
                    {
                        if (!request.memberUnitIds.Contains(member.Id))
                        {
                            await _unitMemberRepository.DeleteUnitMember(request.Id, member.Id, cancellationToken);
                        }
                    }

                    for (int i = 0; i < request.memberUnitIds.Count; i++)
                    {
                        var memberId = request.memberUnitIds[i];
                        var existingMember = await _unitMemberRepository.GetUnitMemberById(request.Id, memberId, cancellationToken);
                        if (existingMember == null)
                        {
                            await _unitMemberRepository.AddUnitMember(new Domain.Entities.UnitMember
                            {
                                UnitId = request.Id,
                                MemberId = memberId,
                                MemberName = request.memberUnitNames[i],
                                MemberFullName = ""
                            }, cancellationToken);
                        }
                    }
                }

                // Save the updated unit
                var result = await _unitRepository.UpdateUnit(unit, cancellationToken);
                
                //
                var listMemberUnits = await _unitMemberRepository.GetAllUnitMember(request.Id, null, cancellationToken);
                var memberUnits = listMemberUnits.Select(x => new { x.MemberId, x.MemberName }).ToList();
                var res = new UnitSimple()
                {
                    Id = unit.Id,
                    TenantId = unit.TenantId,
                    Name = unit.Name,
                    Description = unit.Description,
                    ManagerId = unit.ManagerId,
                    ManagerName = unit.ManagerName,
                    Priority = unit.Priority,
                    DueDate = unit.DueDate,
                    CreatedDate = unit.CreatedDate,
                    ParentUnitId = unit.ParentUnitId,
                    memberUnitIds = memberUnits.Select(x => x.MemberId).ToList(),
                    memberUnitNames = memberUnits.Select(x => x.MemberName).ToList()
                };
                
                return new ResultCustom<UnitSimple>
                {
                    Status = StatusCode.OK,
                    Message = new[] { "Unit updated successfully" },
                    Data = res
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