namespace FamilyManagement.Application.Interfaces;

public interface IDemoDataSeeder
{
    Task EnsureSeededAsync(CancellationToken ct = default);
    Task ResetAndSeedAsync(CancellationToken ct = default);
}
