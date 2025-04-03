using PrefSvc.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PerfSvc.Domain.Entities;

public class Unit
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    public required string Name { get; set; }
    public string? Description { get; set; } = string.Empty;
    public Guid? ManagerId { get; set; }
    public string? ManagerName { get; set; }
    public PriorityUnit? Priority { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public DateTime? DueDate { get; set; }
    [ForeignKey(nameof(Unit))]
    public Guid? ParentUnitId { get; set; }
    public Guid TenantId {set; get;}
    public Guid? TodoId {set; get;}
    public Unit? ParentUnit { get; set; }

    public ICollection<Unit>? SubUnits { get; set; }
    public ICollection<UnitMember>? UnitMembers { get; set; }
    public ICollection<ObjectTB>? Objects {set; get;}
}
