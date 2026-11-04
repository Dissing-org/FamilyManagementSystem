using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Application.DTOs.Freezers;

/// <summary>
/// Data transfer object representing a freezer unit and its compartments.
/// </summary>
/// <param name="Id">The unique identifier of the freezer.</param>
/// <param name="Name">The name or label of the freezer.</param>
/// <param name="Location">The optional physical location of the freezer.</param>
/// <param name="DisplayOrder">The display ordering index of the freezer.</param>
/// <param name="CreatedAt">The timestamp when the freezer was created.</param>
/// <param name="Compartments">The list of compartments belonging to this freezer.</param>
public record FreezerDto(
    Guid Id,
    string Name,
    string? Location,
    int DisplayOrder,
    DateTime CreatedAt,
    IReadOnlyList<FreezerCompartmentDto> Compartments)
{
    /// <summary>
    /// Creates a new <see cref="FreezerDto"/> from a domain <see cref="Freezer"/> entity.
    /// </summary>
    /// <param name="freezer">The domain freezer entity.</param>
    /// <param name="compartmentItemCountLookup">An optional lookup function resolving the item count for each compartment by ID.</param>
    /// <returns>A <see cref="FreezerDto"/> populated with values from the domain entity and ordered compartments.</returns>
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
