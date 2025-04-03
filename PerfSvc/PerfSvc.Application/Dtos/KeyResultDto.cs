using PrefSvc.Domain.Enums;

namespace PerfSvc.Application.Dtos;

public class KeyResultDto
{
    public Guid Id { get; set; }

    public required string Title { get; set; }
    public string? Description { set; get; } = string.Empty;

    public DateTime? CreatedDate { set; get; } = DateTime.Now;
    public DateTime? DueDate { set; get; }

    public PriorityKeyResult? Priority { set; get; } = PriorityKeyResult.None;

    public Guid ObjectTBId { set; get; }

    public List<Guid>? MemberIds { set; get; }
    public List<string?>? MemberNames { set; get; }
}