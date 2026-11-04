using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

/// <summary>
/// Command to delete a compartment from a freezer unit.
/// </summary>
/// <param name="FreezerId">The unique identifier of the owning freezer unit.</param>
/// <param name="CompartmentId">The unique identifier of the compartment to delete.</param>
public record DeleteCompartmentCommand(
    Guid FreezerId,
    Guid CompartmentId);

/// <summary>
/// Handler for processing <see cref="DeleteCompartmentCommand"/> requests.
/// </summary>
public class DeleteCompartmentCommandHandler
{
    private readonly IFreezerRepository _freezerRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteCompartmentCommandHandler"/> class.
    /// </summary>
    /// <param name="freezerRepository">The repository for accessing and modifying freezer aggregate data.</param>
    public DeleteCompartmentCommandHandler(IFreezerRepository freezerRepository)
    {
        _freezerRepository = freezerRepository;
    }

    /// <summary>
    /// Handles deleting a compartment from a freezer unit.
    /// </summary>
    /// <param name="command">The command containing freezer and compartment identifiers.</param>
    /// <param name="ct">The cancellation token to observe while awaiting the operation.</param>
    /// <returns><c>true</c> if the compartment was found and deleted; otherwise, <c>false</c>.</returns>
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
