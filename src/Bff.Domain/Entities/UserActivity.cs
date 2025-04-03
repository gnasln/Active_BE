namespace Bff.Domain.Entities;

public record UserActivity
{
    public int UserId;
    public string UserName;
    public string Activity;
    public DateTime LogTime;
}
