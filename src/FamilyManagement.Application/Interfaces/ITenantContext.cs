namespace FamilyManagement.Application.Interfaces;

/// <summary>
/// Defines the authorization role assigned to a user in the current workspace.
/// </summary>
public enum WorkspaceRole
{
    /// <summary>
    /// Full workspace ownership privileges.
    /// </summary>
    Owner,

    /// <summary>
    /// Sandboxed demo access privileges.
    /// </summary>
    Demo
}

/// <summary>
/// Provides context regarding the active tenant, user, and session.
/// </summary>
public interface ITenantContext
{
    /// <summary>
    /// Gets the role assigned to the user within the current workspace.
    /// </summary>
    WorkspaceRole Role { get; }

    /// <summary>
    /// Gets the email address of the current authenticated user.
    /// </summary>
    string UserEmail { get; }

    /// <summary>
    /// Gets the display name of the current user.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Gets a value indicating whether the current session is a demo workspace.
    /// </summary>
    bool IsDemo => Role == WorkspaceRole.Demo;

    /// <summary>
    /// Gets a value indicating whether the current session is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
}
