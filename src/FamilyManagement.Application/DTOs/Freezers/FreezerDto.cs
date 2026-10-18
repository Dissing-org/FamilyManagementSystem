using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Application.DTOs.Freezers;

public record FreezerDto(
    Guid Id,
    string Name,
    string? Location,
    int DisplayOrder,
    DateTime CreatedAt,
    IReadOnlyList<FreezerCompartmentDto> Compartments)
{
    public static FreezerDto FromDomain(Freezer freezer, Func<Guid, int>? compartmentItemCountLookup = null) =>
        new(
            freezer.Id.Value,
            freezer.Name,
            freezer.Location,
            freezer.DisplayOrder,
            freezer.CreatedAt,
            freezer.Compartments
                .OrderBy(c => c.DisplayOrder)
                .Select(c => FreezerCompartmentDto.FromDomain(c, compartmentItemCountLookup?.Invoke(c.Id.Value) ?? 0))
                .ToList());
}
