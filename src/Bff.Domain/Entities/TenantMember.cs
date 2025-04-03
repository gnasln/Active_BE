using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bff.Domain.Entities;

public class TenantMember {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id {set; get;}
    public Guid UserId {set; get;}
    public required string UserName {set; get;}
    public required string UserFullName {set; get;}

    [ForeignKey(nameof(Tenant))]
    public Guid TenantId {set; get;}
}