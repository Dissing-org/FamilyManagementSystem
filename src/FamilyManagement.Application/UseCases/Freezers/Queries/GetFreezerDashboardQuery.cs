using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Freezers.Queries;

/// <summary>
/// Query to retrieve an aggregated overview dashboard for all freezers and items.
/// </summary>
public record GetFreezerDashboardQuery();

/// <summary>
/// Handler for processing <see cref="GetFreezerDashboardQuery"/> requests.
/// </summary>
public class GetFreezerDashboardQueryHandler
{
    private readonly IFreezerRepository _freezerRepository;
    private readonly IFreezerItemRepository _itemRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetFreezerDashboardQueryHandler"/> class.
    /// </summary>
    /// <param name="freezerRepository">The repository for accessing freezer aggregate data.</param>
    /// <param name="itemRepository">The repository for accessing freezer item inventory data.</param>
    public GetFreezerDashboardQueryHandler(
        IFreezerRepository freezerRepository,
        IFreezerItemRepository itemRepository)
    {
        _freezerRepository = freezerRepository;
        _itemRepository = itemRepository;
    }

    /// <summary>
    /// Handles aggregating metrics, warnings, and categorized items across all freezers into a dashboard view.
    /// </summary>
    /// <param name="query">The query requesting dashboard metrics.</param>
    /// <param name="ct">The cancellation token to observe while awaiting the operation.</param>
    /// <returns>A <see cref="FreezerDashboardDto"/> containing aggregated inventory statistics and urgent action items.</returns>
    public async Task<FreezerDashboardDto> HandleAsync(GetFreezerDashboardQuery query, CancellationToken ct = default)
    {
        var freezers = await _freezerRepository.GetAllWithCompartmentsAsync(ct);
        var items = await _itemRepository.GetAllAsync(ct);
        var now = DateTime.UtcNow;

        var countsByCompartment = items
            .GroupBy(i => i.CompartmentId.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        var freezerDtos = freezers
            .OrderBy(f => f.DisplayOrder)
            .Select(f => FreezerDto.FromDomain(f, compartmentId => countsByCompartment.GetValueOrDefault(compartmentId, 0)))
            .ToList();

        var expiringSoon = items.Where(i => i.EvaluateFreshness(now) == FreshnessStatus.ExpiringSoon).ToList();
        var expired = items.Where(i => i.EvaluateFreshness(now) == FreshnessStatus.Expired).ToList();
        var lowStock = items.Where(i => i.IsLowStock()).ToList();

        var urgent = expired
            .Concat(expiringSoon)
            .Concat(lowStock)
            .DistinctBy(i => i.Id.Value)
            .OrderBy(i => i.ExpiryDate ?? DateTime.MaxValue)
            .Take(10)
            .Select(i => FreezerItemDto.FromDomain(i, now))
            .ToList();

        var itemsByCategory = items
            .GroupBy(i => i.Category.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

        return new FreezerDashboardDto(
            freezerDtos,
            items.Count,
            expiringSoon.Count,
            expired.Count,
            lowStock.Count,
            urgent,
            itemsByCategory);
    }
}
