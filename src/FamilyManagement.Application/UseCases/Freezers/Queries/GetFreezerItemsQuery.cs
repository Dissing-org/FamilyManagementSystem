using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Queries;

public record GetFreezerItemsQuery(
    Guid? CompartmentId = null,
    FreezerCategory? Category = null,
    FreshnessStatus? Freshness = null,
    bool? LowStockOnly = null);

public class GetFreezerItemsQueryHandler
{
    private readonly IFreezerItemRepository _repository;

    public GetFreezerItemsQueryHandler(IFreezerItemRepository repository)
    {
        _repository = repository;
    }

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
