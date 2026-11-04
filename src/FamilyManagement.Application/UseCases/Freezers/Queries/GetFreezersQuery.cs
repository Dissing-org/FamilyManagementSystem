using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Freezers.Queries;

/// <summary>
/// Query to retrieve all freezers and their compartments.
/// </summary>
public record GetFreezersQuery();

/// <summary>
/// Handler for processing <see cref="GetFreezersQuery"/> requests.
/// </summary>
public class GetFreezersQueryHandler
{
    private readonly IFreezerRepository _freezerRepository;
    private readonly IFreezerItemRepository _itemRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetFreezersQueryHandler"/> class.
    /// </summary>
    /// <param name="freezerRepository">The repository for accessing freezer aggregate data.</param>
    /// <param name="itemRepository">The repository for accessing freezer item inventory data.</param>
    public GetFreezersQueryHandler(IFreezerRepository freezerRepository, IFreezerItemRepository itemRepository)
    {
        _freezerRepository = freezerRepository;
        _itemRepository = itemRepository;
    }

    /// <summary>
    /// Handles retrieving all freezers with their compartments and calculated item counts.
    /// </summary>
    /// <param name="query">The query requesting all freezers.</param>
    /// <param name="ct">The cancellation token to observe while awaiting the operation.</param>
    /// <returns>A read-only list of <see cref="FreezerDto"/> ordered by display order.</returns>
    public async Task<IReadOnlyList<FreezerDto>> HandleAsync(GetFreezersQuery query, CancellationToken ct = default)
    {
        var freezers = await _freezerRepository.GetAllWithCompartmentsAsync(ct);
        var items = await _itemRepository.GetAllAsync(ct);

        var countsByCompartment = items
            .GroupBy(i => i.CompartmentId.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        return freezers
            .OrderBy(f => f.DisplayOrder)
            .Select(f => FreezerDto.FromDomain(f, compartmentId => countsByCompartment.GetValueOrDefault(compartmentId, 0)))
            .ToList();
    }
}
