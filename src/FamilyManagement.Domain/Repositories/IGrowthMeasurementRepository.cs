using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

public interface IGrowthMeasurementRepository
{
    Task AddAsync(GrowthMeasurement measurement, CancellationToken cancellationToken = default);
    Task<GrowthMeasurement?> GetByIdAsync(GrowthMeasurementId id, CancellationToken cancellationToken = default);
    Task<List<GrowthMeasurement>> GetByChildIdAsync(ChildId childId, CancellationToken cancellationToken = default);
    Task<GrowthMeasurement?> GetLatestByChildIdAsync(ChildId childId, CancellationToken cancellationToken = default);
    Task UpdateAsync(GrowthMeasurement measurement, CancellationToken cancellationToken = default);
    Task DeleteAsync(GrowthMeasurement measurement, CancellationToken cancellationToken = default);
}
