namespace Bff.Dtos;

public class TenantUserDto
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Role { get; set; }
    public string? Status { get; set; }
}