using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PrefSvc.Domain.Enums;

namespace PerfSvc.Domain.Entities;

public class KeyResult {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set;}

    public required string Title {get; set;}
    public string? Description {set; get;} = string.Empty;
    
    public DateTime? CreatedDate {set; get;}
    public DateTime? DueDate {set; get;}

    public PriorityKeyResult? Priority {set; get;}

    [ForeignKey(nameof(System.Object))]
    public Guid ObjectTBId {set; get;}
    
    public ICollection<KeyResultMember>? KeyResultMembers {set; get;}


}