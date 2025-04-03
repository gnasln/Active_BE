using MediatR;
using NetHelper.Common.Models;
using PerfSvc.Application.Dtos;
using PerfSvc.Application.KeyResultMember.Queries;
using PerfSvc.Infrastructure.Interface.Repository;
using PrefSvc.Domain.Enums;

namespace PerfSvc.Application.KeyResult.Commands;

public class UpdateKeyResultCommand : IRequest<ResultCustom<KeyResultDto>>
{
    public Guid Id { get; set; }

    public required string Title { get; set; }
    public string? Description { set; get; } = string.Empty;

    public DateTime? CreatedDate { set; get; } = DateTime.Now;
    public DateTime? DueDate { set; get; }

    public PriorityKeyResult? Priority { set; get; } = PriorityKeyResult.None;

    public Guid ObjectTBId { set; get; }

    public List<Guid>? MemberIds { set; get; }
    public List<string>? MemberNames { set; get; }
}

public class UpdateKeyResultCommandHandler : IRequestHandler<UpdateKeyResultCommand, ResultCustom<KeyResultDto>>
{
    private readonly IKeyResultRepository _keyResultRepository;
    private readonly ISender _sender;
    private readonly IKeyResultMemberRepository _keyResultMemberRepository;

    public UpdateKeyResultCommandHandler(
        IKeyResultRepository keyResultRepository,
        ISender sender,
        IKeyResultMemberRepository keyResultMemberRepository)
    {
        _keyResultRepository = keyResultRepository;
        _sender = sender;
        _keyResultMemberRepository = keyResultMemberRepository;
    }

    public async Task<ResultCustom<KeyResultDto>> Handle(UpdateKeyResultCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var keyResult = await _keyResultRepository.ReadKeyResult(request.Id, cancellationToken);
            if (keyResult == null)
            {
                return new ResultCustom<KeyResultDto>()
                {
                    Status = StatusCode.NOTFOUND,
                    Message = new[] { "Key result not found" }
                };
            }

            if (request.Title != null) keyResult.Title = request.Title;
            if (request.Description != null) keyResult.Description = request.Description;
            if (request.DueDate != null) keyResult.DueDate = request.DueDate;
            if (request.Priority != null) keyResult.Priority = request.Priority;
            if (request.ObjectTBId != null) keyResult.ObjectTBId = request.ObjectTBId;

            /* c***** */
            var history = new KeyresultHistory
            {
                Id = Guid.NewGuid(),
                ObjectTBId = keyResult.ObjectTBId,
                keyresultid = keyResult.Id,
                Title = keyResult.Title,
                Description = keyResult.Description,
                Priority = keyResult.Priority.ToString(),
                DueDate = keyResult.DueDate,
                ModifiedDate = DateTime.UtcNow.AddHours(7),
            };
            await _keyResultRepository.CreateKeyResultHistory(history, cancellationToken);
            await _keyResultRepository.UpdateKeyResult(keyResult, cancellationToken);

            // Update Member
            if (request.MemberIds != null && request.MemberNames != null)
            {
                var existingMembers = (await _sender.Send(
                    new GetMemberFromKeyResultQuery() { KeyResultId = request.Id },
                    cancellationToken)).Data;
                foreach (var member in existingMembers)
                {
                    if (!request.MemberIds.Contains(member.Id))
                    {
                        await _keyResultMemberRepository.DeleteKeyResultMember(member.Id, cancellationToken);
                    }
                }

                for (int i = 0; i < request.MemberIds.Count; i++)
                {
                    var memberId = request.MemberIds[i];
                    var existingMember = await _keyResultMemberRepository.GetKeyResultMember(memberId, cancellationToken);
                    if (existingMember == null)
                    {
                        await _keyResultMemberRepository.AddKeyResultMember(new Domain.Entities.KeyResultMember()
                        {
                            KeyResultId = request.Id,
                            MemberId = memberId,
                            MemberName = request.MemberNames[i],
                            MemberFullName = ""
                        }, cancellationToken);
                    }
                }
            }

            var listMemberKeyResults = await _keyResultMemberRepository.GetAllKeyResultMember(request.Id, cancellationToken);
            var memberKeyResults = listMemberKeyResults?.Select(x => new { x.MemberId, x.MemberName }).ToList();
            var res = new KeyResultDto()
            {
                Id = keyResult.Id,
                Title = keyResult.Title,
                Description = keyResult.Description,
                Priority = keyResult.Priority,
                CreatedDate = keyResult.CreatedDate,
                DueDate = keyResult.DueDate,
                ObjectTBId = keyResult.ObjectTBId,
                MemberIds = memberKeyResults?.Select(x => x.MemberId).ToList(),
                MemberNames = memberKeyResults?.Select(x => x.MemberName).ToList()
            };
            return new ResultCustom<KeyResultDto>()
            {
                Status = StatusCode.OK,
                Message = new[] { "Key result updated successfully" },
                Data = res
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