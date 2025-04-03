using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bff.Domain.Entities;

public class Tenant {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id {set; get;}
    
    public string Name {set; get;} = string.Empty;
    public Guid Owner {set; get;}
    public string OwnerName {set; get;} = string.Empty;
    public string Description {set; get;} = string.Empty;
    public DateTime CreatedDate {set; get;} = DateTime.Now;
    public bool IsWorkSpacePersonal {set; get;}
    public ICollection<TenantMember>? TenantMembers {set; get;}
}