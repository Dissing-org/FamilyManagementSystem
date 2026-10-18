using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Application.DTOs.Freezers;

public record FreezerCompartmentDto(
    Guid Id,
    Guid FreezerId,
    string Name,
    int DisplayOrder,
    int ItemCount = 0)
{
    public static FreezerCompartmentDto FromDomain(FreezerCompartment compartment, int itemCount = 0) =>
        new(compartment.Id.Value, compartment.FreezerId.Value, compartment.Name, compartment.DisplayOrder, itemCount);
}
