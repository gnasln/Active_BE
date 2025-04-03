using AutoMapper;
using PrefSvc.Domain.Enums;

namespace PerfSvc.Application.Dtos;

public class UnitSimple
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public PriorityUnit? Priority { get; set; }
    public Guid? ParentUnitId { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DueDate { get; set; }
    public required Guid TenantId { get; set; }
    public List<Guid>? memberUnitIds { get; set; }
    public List<string?> memberUnitNames { get; set; }
    public ICollection<Domain.Entities.Unit>? SubUnits { get; init; }

    // AutoMapper
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<Domain.Entities.Unit, UnitSimple>();
        }
    }
}
