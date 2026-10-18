using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

public interface IFreezerItemRepository
{
    Task<IReadOnlyList<FreezerItem>> GetAllAsync(CancellationToken ct = default);
    Task<IReadOnlyList<FreezerItem>> GetByCompartmentIdAsync(FreezerCompartmentId compartmentId, CancellationToken ct = default);
    Task<FreezerItem?> GetByIdAsync(FreezerItemId id, CancellationToken ct = default);
    Task AddAsync(FreezerItem item, CancellationToken ct = default);
    Task UpdateAsync(FreezerItem item, CancellationToken ct = default);
    Task DeleteAsync(FreezerItemId id, CancellationToken ct = default);
    Task AddTransactionAsync(FreezerTransaction transaction, CancellationToken ct = default);
    Task<IReadOnlyList<FreezerTransaction>> GetTransactionsByItemIdAsync(FreezerItemId itemId, CancellationToken ct = default);
}
