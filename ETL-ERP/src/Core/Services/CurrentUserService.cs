using Interface.Services;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Services;

public class CurrentUserService : ICurrentUserService
{
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var value = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(value)) value = httpContextAccessor.HttpContext?.User?.Claims.FirstOrDefault(c => c.Type.Contains("sub"))?.Value;
        UserId = !string.IsNullOrEmpty(value) ? Convert.ToInt64(value) : 0;
        IsAuthenticated = httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }

    public long UserId { get; }
    public bool IsAuthenticated { get; }
}
