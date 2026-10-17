using System.Security.Claims;
using FamilyManagement.Application.Interfaces;

namespace FamilyManagement.Api.Auth;

public class HttpContextTenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpContextTenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public WorkspaceRole Role
    {
        get
        {
            if (!IsAuthenticated)
            {
                return WorkspaceRole.Owner;
            }

            var roleClaim = User?.FindFirstValue(ClaimTypes.Role);
            return string.Equals(roleClaim, nameof(WorkspaceRole.Demo), StringComparison.OrdinalIgnoreCase)
                ? WorkspaceRole.Demo
                : WorkspaceRole.Owner;
        }
    }

    public string UserEmail => IsAuthenticated
        ? User?.FindFirstValue(ClaimTypes.Email) ?? string.Empty
        : string.Empty;

    public string DisplayName => IsAuthenticated
        ? User?.FindFirstValue(ClaimTypes.Name) ?? string.Empty
        : string.Empty;
}
