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

    public void UpdateDetails(string name, int displayOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Compartment name is required.", nameof(name));
        }

        Name = name.Trim();
        DisplayOrder = displayOrder;
    }

    public void UpdateName(string name, int displayOrder) => UpdateDetails(name, displayOrder);
}
