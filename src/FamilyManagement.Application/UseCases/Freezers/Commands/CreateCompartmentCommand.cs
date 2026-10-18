using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

public record CreateCompartmentCommand(
    Guid FreezerId,
    string Name,
    int DisplayOrder = 0);

public class CreateCompartmentCommandHandler
{
    private readonly IFreezerRepository _freezerRepository;

    public CreateCompartmentCommandHandler(IFreezerRepository freezerRepository)
    {
        _freezerRepository = freezerRepository;
    }

    public async Task<FreezerCompartmentDto> HandleAsync(CreateCompartmentCommand command, CancellationToken ct = default)
    {
        var freezer = await _freezerRepository.GetByIdAsync(FreezerId.From(command.FreezerId), ct);
        if (freezer is null)
        {
            throw new KeyNotFoundException($"Freezer with ID '{command.FreezerId}' was not found.");
        }

        var compartment = freezer.AddCompartment(command.Name, command.DisplayOrder);
        await _freezerRepository.UpdateAsync(freezer, ct);
        return FreezerCompartmentDto.FromDomain(compartment, 0);
    }
}
