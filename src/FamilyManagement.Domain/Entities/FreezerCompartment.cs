using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Entities;

public class FreezerCompartment
{
    public FreezerCompartmentId Id { get; private set; }
    public FreezerId FreezerId { get; private set; }
    public string Name { get; private set; }
    public int DisplayOrder { get; private set; }

    private FreezerCompartment()
    {
        Id = null!;
        FreezerId = null!;
        Name = null!;
    }

    private FreezerCompartment(FreezerCompartmentId id, FreezerId freezerId, string name, int displayOrder)
    {
        Id = id;
        FreezerId = freezerId;
        Name = name;
        DisplayOrder = displayOrder;
    }

    /// <summary>
    /// Creates a new <see cref="FreezerCompartment"/> associated with a freezer.
    /// </summary>
    /// <param name="freezerId">The identifier of the parent freezer unit.</param>
    /// <param name="name">The name or label of the compartment.</param>
    /// <param name="displayOrder">The sort order for UI display. Defaults to 0.</param>
    /// <returns>A new <see cref="FreezerCompartment"/> instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="freezerId"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
    public static FreezerCompartment Create(FreezerId freezerId, string name, int displayOrder = 0)
    {
        ArgumentNullException.ThrowIfNull(freezerId);

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Compartment name is required.", nameof(name));
        }

        return new FreezerCompartment(
            FreezerCompartmentId.New(),
            freezerId,
            name.Trim(),
            displayOrder);
    }

    /// <summary>
    /// Updates the name and display order of the compartment.
    /// </summary>
    /// <param name="name">The updated compartment name.</param>
    /// <param name="displayOrder">The updated sort order for UI display.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
    public void UpdateDetails(string name, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Compartment name is required.", nameof(name));
        }

        Name = name.Trim();
        DisplayOrder = displayOrder;
    }

    /// <summary>
    /// Updates the name and display order of the compartment. Alias for <see cref="UpdateDetails"/>.
    /// </summary>
    /// <param name="name">The updated compartment name.</param>
    /// <param name="displayOrder">The updated sort order for UI display.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
    public void UpdateName(string name, int displayOrder) => UpdateDetails(name, displayOrder);
}
