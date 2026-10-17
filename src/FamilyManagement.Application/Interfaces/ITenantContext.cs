namespace FamilyManagement.Application.Interfaces;

public enum WorkspaceRole
{
    Owner,
    Demo
}

public interface ITenantContext
{
    WorkspaceRole Role { get; }
    string UserEmail { get; }
    string DisplayName { get; }
    bool IsDemo => Role == WorkspaceRole.Demo;
    bool IsAuthenticated { get; }
}
