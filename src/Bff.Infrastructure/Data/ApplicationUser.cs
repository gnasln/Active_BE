using Microsoft.AspNetCore.Identity;

namespace Bff.Infrastructure.Data;

public class ApplicationUser : IdentityUser
{
    public string? OTP { set; get; }
    public string? Address { set; get; }
    public string? DisplayName { get; set; }
    public string? Status { get; set; } = "0";
    public string? FullName { get; set; }
    public DateTime? ActivationDate { get; set; } = DateTime.MinValue;
    public string? Tenant { get; set; }
    public DateTime? Birthday { set; get; } = DateTime.MinValue;
    public DateTime? Expires_at { get; set; }
    public DateTime? Updated_at {get; set;}

}
