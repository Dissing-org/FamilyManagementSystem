using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

/// <summary>
/// Repository interface for managing <see cref="FreezerCatalogItem"/> entities.
/// </summary>
public interface IFreezerCatalogRepository
{
    /// <summary>
    /// Retrieves all freezer catalog items.
    /// </summary>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a read-only list of all catalog items.</returns>
    Task<IReadOnlyList<FreezerCatalogItem>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Retrieves a freezer catalog item by its name.
    /// </summary>
    /// <param name="name">The name of the catalog item to search for.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the matching catalog item or <c>null</c> if not found.</returns>
    Task<FreezerCatalogItem?> GetByNameAsync(string name, CancellationToken ct = default);

    /// <summary>
    /// Persists a new freezer catalog item to the repository.
    /// </summary>
    /// <param name="item">The catalog item entity to add.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the newly added catalog item.</returns>
    Task<FreezerCatalogItem> AddAsync(FreezerCatalogItem item, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing freezer catalog item in the repository.
    /// </summary>
    /// <param name="item">The catalog item entity with updated values.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(FreezerCatalogItem item, CancellationToken ct = default);

    /// <summary>
    /// Deletes a freezer catalog item by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the catalog item to delete.</param>
    /// <param name="ct">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(FreezerCatalogItemId id, CancellationToken ct = default);
}
