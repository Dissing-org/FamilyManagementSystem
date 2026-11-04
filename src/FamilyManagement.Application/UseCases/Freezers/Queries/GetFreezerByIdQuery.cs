using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Queries;

/// <summary>
/// Query to retrieve a freezer unit by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the freezer to retrieve.</param>
public record GetFreezerByIdQuery(Guid Id);

/// <summary>
/// Handler for processing <see cref="GetFreezerByIdQuery"/> requests.
/// </summary>
public class GetFreezerByIdQueryHandler
{
    private readonly IFreezerRepository _freezerRepository;
    private readonly IFreezerItemRepository _itemRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetFreezerByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="freezerRepository">The repository for accessing freezer aggregate data.</param>
    /// <param name="itemRepository">The repository for accessing freezer item inventory data.</param>
    public GetFreezerByIdQueryHandler(IFreezerRepository freezerRepository, IFreezerItemRepository itemRepository)
    {
        _freezerRepository = freezerRepository;
        _itemRepository = itemRepository;
    }

    /// <summary>
    /// Handles retrieving a freezer by its unique identifier, including compartment item counts.
    /// </summary>
    /// <param name="query">The query containing the freezer ID to retrieve.</param>
    /// <param name="ct">The cancellation token to observe while awaiting the operation.</param>
    /// <returns>A <see cref="FreezerDto"/> representing the freezer if found; otherwise, <c>null</c>.</returns>
    public async Task<FreezerDto?> HandleAsync(GetFreezerByIdQuery query, CancellationToken ct = default)
    {
        var freezer = await _freezerRepository.GetByIdAsync(FreezerId.From(query.Id), ct);
        if (freezer is null)
        {
            return null;
        }

        var items = await _itemRepository.GetAllAsync(ct);
        var countsByCompartment = items
            .GroupBy(i => i.CompartmentId.Value)
            .ToDictionary(g => g.Key, g => g.Count());

        return FreezerDto.FromDomain(freezer, compartmentId => countsByCompartment.GetValueOrDefault(compartmentId, 0));
    }
}
