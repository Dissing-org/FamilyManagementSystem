using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Freezers.Queries;

public record GetFreezersQuery();

public class GetFreezersQueryHandler
{
    private readonly IFreezerRepository _freezerRepository;
    private readonly IFreezerItemRepository _itemRepository;

    public GetFreezersQueryHandler(IFreezerRepository freezerRepository, IFreezerItemRepository itemRepository)
    {
        _freezerRepository = freezerRepository;
        _itemRepository = itemRepository;
    }

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
