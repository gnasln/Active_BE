using System.Security.Claims;

using Bff.Application.Common.Interfaces;

namespace Bff.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : IUser
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public string? Id => _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");
    
    public string? UserName => _httpContextAccessor.HttpContext?.User?.FindFirstValue("name");

    /* recheck later 
    public string? Id1 => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? UserName1 => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);
    */
}
