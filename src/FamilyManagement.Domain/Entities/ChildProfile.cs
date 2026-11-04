using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Entities;

public class ChildProfile
{
    public ChildId Id { get; private set; }
    public string FirstName { get; private set; }
    public string? LastName { get; private set; }
    public DateTime DateOfBirth { get; private set; }
    public Gender Gender { get; private set; }
    public string? AvatarUrl { get; private set; }
    public ChildWardrobeSizes CurrentSizes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    // EF Core parameterless constructor
    private ChildProfile()
    {
        Id = null!;
        FirstName = null!;
        CurrentSizes = ChildWardrobeSizes.Empty;
    }

    private ChildProfile(
        ChildId id,
        string firstName,
        string? lastName,
        DateTime dateOfBirth,
        Gender gender,
        string? avatarUrl,
        ChildWardrobeSizes currentSizes,
        DateTime createdAt)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = DateTime.SpecifyKind(dateOfBirth, DateTimeKind.Utc);
        Gender = gender;
        AvatarUrl = avatarUrl;
        CurrentSizes = currentSizes;
        CreatedAt = createdAt;
    }

    /// <summary>
    /// Creates a new <see cref="ChildProfile"/> instance with the specified personal details and initial wardrobe sizes.
    /// </summary>
    /// <param name="firstName">The child's first name.</param>
    /// <param name="lastName">The child's optional last name.</param>
    /// <param name="dateOfBirth">The child's date of birth.</param>
    /// <param name="gender">The child's gender.</param>
    /// <param name="avatarUrl">An optional URL referencing the child's avatar image.</param>
    /// <param name="initialSizes">Optional initial wardrobe size specifications.</param>
    /// <returns>A new <see cref="ChildProfile"/> instance.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="firstName"/> is null or whitespace, or when <paramref name="dateOfBirth"/> is in the future.</exception>
    public static ChildProfile Create(
        string firstName,
        string? lastName,
        DateTime dateOfBirth,
        Gender gender,
        string? avatarUrl = null,
        ChildWardrobeSizes? initialSizes = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name is required.", nameof(firstName));
        }

        if (dateOfBirth.Date > DateTime.UtcNow.Date.AddDays(1))
        {
            throw new ArgumentException("Date of birth cannot be in the future.", nameof(dateOfBirth));
        }

        return new ChildProfile(
            ChildId.New(),
            firstName.Trim(),
            lastName?.Trim(),
            dateOfBirth,
            gender,
            avatarUrl?.Trim(),
            initialSizes ?? ChildWardrobeSizes.Empty,
            DateTime.UtcNow);
    }

    /// <summary>
    /// Updates the child's personal profile information.
    /// </summary>
    /// <param name="firstName">The updated first name.</param>
    /// <param name="lastName">The updated optional last name.</param>
    /// <param name="dateOfBirth">The updated date of birth.</param>
    /// <param name="gender">The updated gender.</param>
    /// <param name="avatarUrl">The updated optional avatar image URL.</param>
    /// <exception cref="ArgumentException">Thrown when <paramref name="firstName"/> is null or whitespace, or when <paramref name="dateOfBirth"/> is in the future.</exception>
    public void UpdateProfile(
        string firstName,
        string? lastName,
        DateTime dateOfBirth,
        Gender gender,
        string? avatarUrl)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            throw new ArgumentException("First name is required.", nameof(firstName));
        }

        if (dateOfBirth.Date > DateTime.UtcNow.Date.AddDays(1))
        {
            throw new ArgumentException("Date of birth cannot be in the future.", nameof(dateOfBirth));
        }

        FirstName = firstName.Trim();
        LastName = lastName?.Trim();
        DateOfBirth = DateTime.SpecifyKind(dateOfBirth, DateTimeKind.Utc);
        Gender = gender;
        AvatarUrl = avatarUrl?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the child's current wardrobe sizes.
    /// </summary>
    /// <param name="sizes">The updated wardrobe size specifications.</param>
    public void UpdateSizes(ChildWardrobeSizes sizes)
    {
        CurrentSizes = sizes ?? ChildWardrobeSizes.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Calculates the child's age relative to a given date or current time.
    /// </summary>
    /// <param name="asOfDate">The optional reference date to calculate age against. Defaults to current UTC date and time if null.</param>
    /// <returns>A <see cref="ChildAge"/> representing the calculated age in years, months, and total months.</returns>
    public ChildAge GetAge(DateTime? asOfDate = null)
    {
        return ChildAge.Calculate(DateOfBirth, asOfDate ?? DateTime.UtcNow);
    }
}
