using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

public interface IGasReadingRepository
{
    Task<GasReading?> GetByIdAsync(GasReadingId id, CancellationToken ct = default);
    Task<IReadOnlyList<GasReading>> GetAllAsync(CancellationToken ct = default);
    Task AddAsync(GasReading reading, CancellationToken ct = default);
    Task UpdateAsync(GasReading reading, CancellationToken ct = default);
    Task DeleteAsync(GasReading reading, CancellationToken ct = default);
}
