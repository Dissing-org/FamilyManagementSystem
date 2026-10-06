using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Commands;

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

public class CreateChildProfileCommandHandler
{
    private readonly IChildProfileRepository _repository;

    public CreateChildProfileCommandHandler(IChildProfileRepository repository)
    {
        _repository = repository;
    }

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
