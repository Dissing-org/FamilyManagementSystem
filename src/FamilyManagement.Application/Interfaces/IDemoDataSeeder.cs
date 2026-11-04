namespace FamilyManagement.Application.Interfaces;

/// <summary>
/// Service contract for seeding and resetting application demo data.
/// </summary>
public interface IDemoDataSeeder
{
    /// <summary>
    /// Ensures demo data is seeded into the database if not already present.
    /// </summary>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task EnsureSeededAsync(CancellationToken ct = default);

    /// <summary>
    /// Resets all application data and re-seeds fresh sample demo data.
    /// </summary>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task ResetAndSeedAsync(CancellationToken ct = default);
}
