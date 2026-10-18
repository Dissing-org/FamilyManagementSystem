using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

public interface IFreezerRepository
{
    Task<IReadOnlyList<Freezer>> GetAllWithCompartmentsAsync(CancellationToken ct = default);
    Task<Freezer?> GetByIdAsync(FreezerId id, CancellationToken ct = default);
    Task AddAsync(Freezer freezer, CancellationToken ct = default);
    Task UpdateAsync(Freezer freezer, CancellationToken ct = default);
    Task DeleteAsync(FreezerId id, CancellationToken ct = default);
}
