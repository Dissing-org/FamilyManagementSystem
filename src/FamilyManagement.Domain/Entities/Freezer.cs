using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Entities;

public class Freezer
{
    public FreezerId Id { get; private set; }
    public string Name { get; private set; }
    public string? Location { get; private set; }
    public int DisplayOrder { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<FreezerCompartment> _compartments = new();
    public IReadOnlyCollection<FreezerCompartment> Compartments => _compartments.AsReadOnly();

    private Freezer()
    {
        Id = null!;
        Name = null!;
    }

    private Freezer(FreezerId id, string name, string? location, int displayOrder, DateTime createdAt)
    {
        Id = id;
        Name = name;
        Location = location;
        DisplayOrder = displayOrder;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Creates a new <see cref="Freezer"/> unit.
    /// </summary>
    /// <param name="name">The name or label of the freezer unit.</param>
    /// <param name="location">The optional physical location of the freezer unit.</param>
    /// <param name="displayOrder">The sort order for UI display. Defaults to 0.</param>
    /// <returns>A new <see cref="Freezer"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
    public static Freezer Create(string name, string? location = null, int displayOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Freezer name is required.", nameof(name));
        }

        return new Freezer(
            FreezerId.New(),
            name.Trim(),
            location?.Trim(),
            displayOrder,
            DateTime.UtcNow);
    }

    /// <summary>
    /// Adds a new compartment or drawer to this freezer unit.
    /// </summary>
    /// <param name="name">The name or label of the compartment.</param>
    /// <param name="displayOrder">The sort order for UI display. Defaults to 0.</param>
    /// <returns>The newly created and added <see cref="FreezerCompartment"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
    public FreezerCompartment AddCompartment(string name, int displayOrder = 0)
    {
        var compartment = FreezerCompartment.Create(Id, name, displayOrder);
        _compartments.Add(compartment);
        return compartment;
    }

    /// <summary>
    /// Removes a compartment from this freezer unit by its identifier.
    /// </summary>
    /// <param name="compartmentId">The identifier of the compartment to remove.</param>
    public void RemoveCompartment(FreezerCompartmentId compartmentId)
    {
        var compartment = _compartments.FirstOrDefault(c => c.Id == compartmentId);
        if (compartment != null)
        {
            _compartments.Remove(compartment);
        }
    }

    /// <summary>
    /// Updates the descriptive details and display ordering of the freezer unit.
    /// </summary>
    /// <param name="name">The updated name of the freezer.</param>
    /// <param name="location">The updated optional physical location.</param>
    /// <param name="displayOrder">The updated sort order for UI display.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is null or whitespace.</exception>
    public void UpdateDetails(string name, string? location, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Freezer name is required.", nameof(name));
        }

        Name = name.Trim();
        Location = location?.Trim();
        DisplayOrder = displayOrder;
    }
}
