using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

public record DeleteCompartmentCommand(
    Guid FreezerId,
    Guid CompartmentId);

public class DeleteCompartmentCommandHandler
{
    private readonly IFreezerRepository _freezerRepository;

    public DeleteCompartmentCommandHandler(IFreezerRepository freezerRepository)
    {
        _freezerRepository = freezerRepository;
    }

    public async Task<bool> HandleAsync(DeleteCompartmentCommand command, CancellationToken ct = default)
    {
        var freezer = await _freezerRepository.GetByIdAsync(FreezerId.From(command.FreezerId), ct);
        if (freezer is null)
        {
            return false;
        }

        var compartmentId = FreezerCompartmentId.From(command.CompartmentId);
        freezer.RemoveCompartment(compartmentId);
        await _freezerRepository.UpdateAsync(freezer, ct);
        return true;
    }
}
