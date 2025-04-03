using PrefSvc.Domain.Enums;

namespace PerfSvc.Application.Dtos;

public class ObjectDtos
{
    public required Guid Id { get; set;}
    public required string Title { get; set;}
    public string? Description { set; get;} = string.Empty;
    public PriorityObject? Priority { get; set; } = PriorityObject.None;
    public DateTime? CreatedDate {get; set;} = DateTime.Now;
    public DateTime? UpdatedDate {get; set;}
    public DateTime? DueDate { get; set;}
    public Guid UnitId {get; set;}
    public string? UnitName {get; set;}
    public List<Guid>? MemberIds {get; set;}
    public List<string?> MemberNames {get; set;}
}