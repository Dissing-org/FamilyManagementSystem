using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Queries;

public record GetFreezerByIdQuery(Guid Id);

public class GetFreezerByIdQueryHandler
{
    private readonly IFreezerRepository _freezerRepository;
    private readonly IFreezerItemRepository _itemRepository;

    public GetFreezerByIdQueryHandler(IFreezerRepository freezerRepository, IFreezerItemRepository itemRepository)
    {
        _freezerRepository = freezerRepository;
        _itemRepository = itemRepository;
    }

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
