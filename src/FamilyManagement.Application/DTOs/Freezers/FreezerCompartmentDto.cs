using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Application.DTOs.Freezers;

/// <summary>
/// Data transfer object representing a compartment or drawer within a freezer unit.
/// </summary>
/// <param name="Id">The unique identifier of the compartment.</param>
/// <param name="FreezerId">The unique identifier of the owning freezer unit.</param>
/// <param name="Name">The name or label of the compartment.</param>
/// <param name="DisplayOrder">The display ordering index of the compartment.</param>
/// <param name="ItemCount">The total count of items stored in this compartment.</param>
public record FreezerCompartmentDto(
    Guid Id,
    Guid FreezerId,
    string Name,
    int DisplayOrder,
    int ItemCount = 0)
{
    /// <summary>
    /// Creates a new <see cref="FreezerCompartmentDto"/> from a domain <see cref="FreezerCompartment"/> entity.
    /// </summary>
    /// <param name="compartment">The domain freezer compartment entity.</param>
    /// <param name="itemCount">The number of inventory items stored in this compartment. Defaults to 0.</param>
    /// <returns>A <see cref="FreezerCompartmentDto"/> populated with values from the domain entity.</returns>
    public static FreezerCompartmentDto FromDomain(FreezerCompartment compartment, int itemCount = 0) =>
        new(compartment.Id.Value, compartment.FreezerId.Value, compartment.Name, compartment.DisplayOrder, itemCount);
}
