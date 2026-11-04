using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

/// <summary>
/// Command to create a new compartment within an existing freezer unit.
/// </summary>
/// <param name="FreezerId">The unique identifier of the target freezer.</param>
/// <param name="Name">The name or label of the compartment to add.</param>
/// <param name="DisplayOrder">The display ordering index for the compartment.</param>
public record CreateCompartmentCommand(
    Guid FreezerId,
    string Name,
    int DisplayOrder = 0);

/// <summary>
/// Handler for processing <see cref="CreateCompartmentCommand"/> requests.
/// </summary>
public class CreateCompartmentCommandHandler
{
    private readonly IFreezerRepository _freezerRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateCompartmentCommandHandler"/> class.
    /// </summary>
    /// <param name="freezerRepository">The repository for accessing and modifying freezer aggregate data.</param>
    public CreateCompartmentCommandHandler(IFreezerRepository freezerRepository)
    {
        _freezerRepository = freezerRepository;
    }

    /// <summary>
    /// Handles creating a new compartment in a freezer unit.
    /// </summary>
    /// <param name="command">The command containing new compartment details.</param>
    /// <param name="ct">The cancellation token to observe while awaiting the operation.</param>
    /// <returns>A <see cref="FreezerCompartmentDto"/> representing the newly created compartment.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the freezer with the specified ID is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when the compartment name is null or whitespace.</exception>
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
