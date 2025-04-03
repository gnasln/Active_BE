namespace Bff.Dtos;

public record GetTenantMemberQuery
{
    public bool? IncludeAllMemberForUnit { get; init; } = true;
    public Guid? UnitId { get; init; }
}
