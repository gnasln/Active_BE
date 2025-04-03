using System.ComponentModel.DataAnnotations;

namespace Bff.Dtos;

public record UserDto
{
    public required Guid Id { get; init; }
    public string? UserName { get; set; }
    public string? TenantName { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
    [Phone]
    public string? NumberPhone { get; set; }
    public string? RoleUser { get; set; }
    public string? status { get; set; }
    public DateTime? ActivationDate { get; set; }
}