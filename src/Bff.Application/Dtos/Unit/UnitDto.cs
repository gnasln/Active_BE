namespace Bff.Application.Dtos.Units;

public enum Priority
{
    None = 0,
    Low = 1,
    Medium = 2,
    High = 3
}

public class UnitDto
{
    public required string Name { get; set; }
    public string? Description { get; set; } = string.Empty;
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public Priority Priority { set; get; }
    public DateTime? DueDate { get; set; }

    public Guid? ParentUnitId { get; set; }
    public Guid TenantId { set; get; }
}