
namespace Bff.Application.Dtos.Tenants;

public class TenantRespone
{
    public Guid Id { set; get; }

    public required string Name { set; get; } = string.Empty;
    public Guid Owner { set; get; }
    public string OwnerName { set; get; } = string.Empty;
    public string Description { set; get; } = string.Empty;
    public DateTime CreatedDate { set; get; }
    public List<Guid>? memberIds { set; get; }
    public List<string?>? memberNames { set; get; }
    public bool IsWorkSpacePersonal {set; get;}
}