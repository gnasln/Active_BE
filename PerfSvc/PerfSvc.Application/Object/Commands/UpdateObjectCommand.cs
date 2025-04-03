using AutoMapper;
using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Application.Dtos;
using PerfSvc.Application.Object.Common;
using PerfSvc.Application.ObjectMember.Queries;
using PerfSvc.Infrastructure.Interface.Repository;
using PrefSvc.Domain.Enums;

namespace PerfSvc.Application.Object.Commands;

public record UpdateObjectCommand : IRequest<ResultCustom<ObjectDtos>>
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { set; get; } = string.Empty;
    public PriorityObject? Priority { get; set; } = PriorityObject.None;
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; } = DateTime.Now;
    public DateTime? DueDate { get; set; }
    public Guid UnitId { get; set; }
    public List<Guid>? MemberIds { get; set; }
    public List<string>? MemberNames { get; set; }

    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UpdateObjectCommand, ObjectDtos>();
        }
    }
}

public class UpdateObjectCommandHandle(
    IObjectRepository objectRepository,
    ISender sender,
    IUnitRepository unitRepository,
    IObjectMemberRepository objectMemberRepository,
    AuthorizationChecker authorizationChecker) : IRequestHandler<UpdateObjectCommand, ResultCustom<ObjectDtos>>
{
    private readonly IObjectRepository _objectRepository = objectRepository;
    private readonly ISender _sender = sender;
    private readonly IUnitRepository _unitRepository = unitRepository;
    private readonly IObjectMemberRepository _objectMemberRepository = objectMemberRepository;
    private readonly AuthorizationChecker _authorizationChecker = authorizationChecker;

    public async Task<ResultCustom<ObjectDtos>> Handle(UpdateObjectCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var unit = await _unitRepository.GetUnitById(request.UnitId, cancellationToken);
            // Check authorization
            var isAuthorized =
                await _authorizationChecker.IsAuthorized(request.UnitId, unit.TenantId, cancellationToken);
            if (!isAuthorized)
            {
                return new ResultCustom<ObjectDtos>()
                {
                    Status = StatusCode.FORBIDDEN,
                    Message = new[] { "Forbidden" }
                };
            }

            // Retrieve the unit by its ID
            var objectTB = await _objectRepository.ReadObject(request.Id, cancellationToken);
            if (objectTB == null)
                return new ResultCustom<ObjectDtos>
                {
                    Status = StatusCode.NOTFOUND,
                    Message = new[] { "Unit not found" }
                };

            // Update object properties
            if (request.Title != null) objectTB.Title = request.Title;
            if (request.Description != null) objectTB.Description = request.Description;
            if (request.DueDate != null) objectTB.DueDate = request.DueDate;
            if (request.Priority != null) objectTB.Priority = request.Priority;

            // Update members
            if (request.MemberIds != null && request.MemberNames != null)
            {
                var existingMembers = (await _sender.Send(new GetMemberFromObjectQuery() { ObjectTBId = request.Id },
                    cancellationToken)).Data;
                foreach (var member in existingMembers)
                {
                    if (!request.MemberIds.Contains(member.Id))
                    {
                        await _objectMemberRepository.RemoveMemberFromObject(member.Id, cancellationToken);
                    }
                }

                for (int i = 0; i < request.MemberIds.Count; i++)
                {
                    var memberId = request.MemberIds[i];
                    var existingMember =
                        await _objectMemberRepository.GetMemberFromObject(request.Id, memberId, cancellationToken);
                    if (existingMember == null)
                    {
                        await _objectMemberRepository.AddMemberToObject(new Domain.Entities.ObjectMember()
                        {
                            ObjectTBId = request.Id,
                            MemberId = memberId,
                            MemberName = request.MemberNames[i],
                            MemberFullName = ""
                        }, cancellationToken);
                    }
                }
            }

            /* 
            c**** 
            */
            var historys = new ObjectHistory
            {
                Id = Guid.NewGuid(),
                UnitId = unit.Id,
                objecttbid = objectTB.Id,
                Title = objectTB.Title,
                Description = objectTB.Description,
                Priority = objectTB.Priority.ToString(),
                DueDate = objectTB.DueDate,
                ModifiedDate = DateTime.UtcNow.AddHours(7)
            };
            await _objectRepository.CreateObjectHistory(historys, cancellationToken);
            /*close C**** */
            // Save the updated unit
            var result = await _objectRepository.UpdateObject(objectTB, cancellationToken);
            var listMemberUnits =
                await _objectMemberRepository.GetAllMemberFromObject(request.Id, "", cancellationToken);
            var memberUnits = listMemberUnits.Select(x => new { x.MemberId, x.MemberName }).ToList();
            var res = new ObjectDtos()
            {
                Id = objectTB.Id,
                Title = objectTB.Title,
                Description = objectTB.Description,
                Priority = objectTB.Priority,
                DueDate = objectTB.DueDate,
                UnitId = objectTB.UnitId,
                CreatedDate = objectTB.CreatedDate,
                UpdatedDate = objectTB.UpdatedDate,
                MemberIds = memberUnits.Select(x => x.MemberId).ToList(),
                MemberNames = memberUnits.Select(x => x.MemberName).ToList()
            };

            return new ResultCustom<ObjectDtos>()
            {
                Status = StatusCode.OK,
                Message = new[] { "Object created successfully" },
                Data = res
            };
        }
        catch (Exception ex)
        {
            return new ResultCustom<ObjectDtos>()
            {
                Status = StatusCode.INTERNALSERVERERROR,
                Message = new[] { "Error while creating object" }
            };
        }
    }
}