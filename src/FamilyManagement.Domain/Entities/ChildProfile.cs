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

    public void UpdateSizes(ChildWardrobeSizes sizes)
    {
        CurrentSizes = sizes ?? ChildWardrobeSizes.Empty;
        UpdatedAt = DateTime.UtcNow;
    }

    public ChildAge GetAge(DateTime? asOfDate = null)
    {
        return ChildAge.Calculate(DateOfBirth, asOfDate ?? DateTime.UtcNow);
    }
}
