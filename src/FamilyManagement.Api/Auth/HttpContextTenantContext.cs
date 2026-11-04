using System.Security.Claims;
using FamilyManagement.Application.Interfaces;

namespace FamilyManagement.Api.Auth;

/// <summary>
/// Provides workspace tenant and user identity context extracted from the active HTTP request.
/// </summary>
public class HttpContextTenantContext : ITenantContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpContextTenantContext"/> class.
    /// </summary>
    /// <param name="httpContextAccessor">The HTTP context accessor used to access request user identity.</param>
    public HttpContextTenantContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    /// <summary>
    /// Gets a value indicating whether the current request user is authenticated.
    /// </summary>
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    /// <summary>
    /// Gets the workspace role of the active user, defaulting to Owner if unauthenticated or demo role claim is not matched.
    /// </summary>
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

    /// <summary>
    /// Gets the email address of the authenticated user, or an empty string if unauthenticated.
    /// </summary>
    public string UserEmail => IsAuthenticated
        ? User?.FindFirstValue(ClaimTypes.Email) ?? string.Empty
        : string.Empty;

    /// <summary>
    /// Gets the display name of the authenticated user, or an empty string if unauthenticated.
    /// </summary>
    public string DisplayName => IsAuthenticated
        ? User?.FindFirstValue(ClaimTypes.Name) ?? string.Empty
        : string.Empty;
}
