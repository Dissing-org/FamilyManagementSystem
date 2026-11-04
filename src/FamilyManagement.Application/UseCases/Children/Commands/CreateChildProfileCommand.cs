using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Commands;

/// <summary>
/// Command to create a new child profile.
/// </summary>
/// <param name="FirstName">The first name of the child.</param>
/// <param name="LastName">The optional last name of the child.</param>
/// <param name="DateOfBirth">The date of birth of the child.</param>
/// <param name="Gender">The gender of the child.</param>
/// <param name="AvatarUrl">The optional avatar image URL.</param>
/// <param name="ClothesSize">The optional clothes size.</param>
/// <param name="ShoeSize">The optional shoe size.</param>
/// <param name="HatSize">The optional hat size.</param>
/// <param name="DiaperSize">The optional diaper size.</param>
public record CreateChildProfileCommand(
    string FirstName,
    string? LastName,
    DateTime DateOfBirth,
    Gender Gender,
    string? AvatarUrl = null,
    string? ClothesSize = null,
    string? ShoeSize = null,
    string? HatSize = null,
    string? DiaperSize = null);

/// <summary>
/// Handler for creating a new child profile.
/// </summary>
public class CreateChildProfileCommandHandler
{
    private readonly IChildProfileRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateChildProfileCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The child profile repository.</param>
    public CreateChildProfileCommandHandler(IChildProfileRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles the creation of a new child profile and persists it.
    /// </summary>
    /// <param name="command">The command containing the child profile details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the created <see cref="ChildProfileDto"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when profile validation fails.</exception>
    public async Task<ChildProfileDto> HandleAsync(
        CreateChildProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var sizes = new ChildWardrobeSizes(
            command.ClothesSize,
            command.ShoeSize,
            command.HatSize,
            command.DiaperSize);

        var child = ChildProfile.Create(
            command.FirstName,
            command.LastName,
            command.DateOfBirth,
            command.Gender,
            command.AvatarUrl,
            sizes);

        await _repository.AddAsync(child, cancellationToken);

        return ChildProfileDto.FromDomain(child);
    }
}
