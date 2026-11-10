using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Domain.Repositories;

public interface IGasPricingRepository
{
    Task<GasPricingSettings> GetSettingsAsync(CancellationToken ct = default);
    Task SaveSettingsAsync(GasPricingSettings settings, CancellationToken ct = default);
    Task<GasPriceSnapshot?> GetSnapshotAsync(int year, int month, CancellationToken ct = default);
    Task<IReadOnlyList<GasPriceSnapshot>> GetAllSnapshotsAsync(CancellationToken ct = default);
    Task SaveSnapshotAsync(GasPriceSnapshot snapshot, CancellationToken ct = default);
}
