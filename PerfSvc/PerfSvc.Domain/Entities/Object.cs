using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PrefSvc.Domain.Enums;

namespace PerfSvc.Domain.Entities;

public class ObjectTB {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set;}
    
    public required string Title { get; set;}
    public string? Description { set; get;} = string.Empty;
    public PriorityObject? Priority { get; set;}
    public DateTime? CreatedDate {get; set;}
    public DateTime? UpdatedDate {get; set;}
    public DateTime? DueDate { get; set;}

    [ForeignKey(nameof(Unit))]
    public Guid UnitId {get; set;}

    public ICollection<KeyResult>? KeyResults {get; set;}
    public ICollection<ObjectMember>? ObjectMembers {get; set;}

}