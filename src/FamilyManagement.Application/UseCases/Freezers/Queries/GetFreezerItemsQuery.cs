using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Queries;

/// <summary>
/// Query to retrieve freezer items based on optional filtering criteria.
/// </summary>
/// <param name="CompartmentId">Optional compartment ID to filter items by.</param>
/// <param name="Category">Optional category to filter items by.</param>
/// <param name="Freshness">Optional freshness status to filter items by.</param>
/// <param name="LowStockOnly">Optional flag to filter only items that are low in stock.</param>
public record GetFreezerItemsQuery(
    Guid? CompartmentId = null,
    FreezerCategory? Category = null,
    FreshnessStatus? Freshness = null,
    bool? LowStockOnly = null);

/// <summary>
/// Handler for processing <see cref="GetFreezerItemsQuery"/> requests.
/// </summary>
public class GetFreezerItemsQueryHandler
{
    private readonly IFreezerItemRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetFreezerItemsQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository for accessing freezer item inventory data.</param>
    public GetFreezerItemsQueryHandler(IFreezerItemRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles retrieving freezer items matching the specified filter criteria, ordered by expiration date.
    /// </summary>
    /// <param name="query">The query containing optional filter parameters.</param>
    /// <param name="ct">The cancellation token to observe while awaiting the operation.</param>
    /// <returns>A read-only list of <see cref="FreezerItemDto"/> matching the filter criteria.</returns>
    public async Task<IReadOnlyList<FreezerItemDto>> HandleAsync(GetFreezerItemsQuery query, CancellationToken ct = default)
    {
        var items = query.CompartmentId.HasValue
            ? await _repository.GetByCompartmentIdAsync(FreezerCompartmentId.From(query.CompartmentId.Value), ct)
            : await _repository.GetAllAsync(ct);

        var now = DateTime.UtcNow;
        var filtered = items.AsEnumerable();

        if (query.Category.HasValue)
        {
            filtered = filtered.Where(i => i.Category == query.Category.Value);
        }

        if (query.Freshness.HasValue)
        {
            filtered = filtered.Where(i => i.EvaluateFreshness(now) == query.Freshness.Value);
        }

        if (query.LowStockOnly == true)
        {
            filtered = filtered.Where(i => i.IsLowStock());
        }

        return filtered
            .OrderBy(i => i.ExpiryDate)
            .Select(i => FreezerItemDto.FromDomain(i, now))
            .ToList();
    }
}
