using TodoSvc.Domain.Enums;

namespace TodoSvc.Application.Dtos;

public class DailyTaskDto
{
    public Guid Id {get; set;}

    public required string? Title {set; get;}
    public PriorityLevel Priority {get; set;}
    public DateTime? CreatedDate {set; get;}
    public DateTime? DueDate {set; get;}
    public Guid? TenantId {set; get;}
    public string? TenantName {set; get;}
    public Guid? TodoOwnerId {set; get;}
    public string? TodoOwnerName {set; get;}
}