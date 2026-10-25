using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

public interface IFreezerCatalogRepository
{
    Task<IReadOnlyList<FreezerCatalogItem>> GetAllAsync(CancellationToken ct = default);
    Task<FreezerCatalogItem?> GetByNameAsync(string name, CancellationToken ct = default);
    Task<FreezerCatalogItem> AddAsync(FreezerCatalogItem item, CancellationToken ct = default);
    Task UpdateAsync(FreezerCatalogItem item, CancellationToken ct = default);
    Task DeleteAsync(FreezerCatalogItemId id, CancellationToken ct = default);
}
