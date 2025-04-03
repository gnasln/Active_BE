namespace Bff.Dtos;

public class SimpleUserInfo
{
    public Guid UserId { get; set; }
    public required string UserName { get; set; }
    public string? FullName { get; set; }
}
