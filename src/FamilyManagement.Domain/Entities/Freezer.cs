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

    public FreezerCompartment AddCompartment(string name, int displayOrder = 0)
    {
        var compartment = FreezerCompartment.Create(Id, name, displayOrder);
        _compartments.Add(compartment);
        return compartment;
    }

    public void RemoveCompartment(FreezerCompartmentId compartmentId)
    {
        var compartment = _compartments.FirstOrDefault(c => c.Id == compartmentId);
        if (compartment != null)
        {
            _compartments.Remove(compartment);
        }
    }

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
