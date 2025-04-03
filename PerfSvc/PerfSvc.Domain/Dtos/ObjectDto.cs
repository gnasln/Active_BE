using PrefSvc.Domain.Enums;

namespace PerfSvc.Domain.Dtos;

public class ObjectDto
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
}