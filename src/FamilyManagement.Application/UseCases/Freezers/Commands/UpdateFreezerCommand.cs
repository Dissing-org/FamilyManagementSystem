using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

/// <summary>
/// Command to update the details and display order of a freezer unit.
/// </summary>
/// <param name="Id">The unique identifier of the freezer to update.</param>
/// <param name="Name">The updated name or label of the freezer.</param>
/// <param name="Location">The updated optional physical location.</param>
/// <param name="DisplayOrder">The updated display ordering index.</param>
public record UpdateFreezerCommand(
    Guid Id,
    string Name,
    string? Location = null,
    int DisplayOrder = 0);

/// <summary>
/// Handler for processing <see cref="UpdateFreezerCommand"/> requests.
/// </summary>
public class UpdateFreezerCommandHandler
{
    private readonly IFreezerRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateFreezerCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository for accessing and modifying freezer aggregate data.</param>
    public UpdateFreezerCommandHandler(IFreezerRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles updating the details of an existing freezer unit.
    /// </summary>
    /// <param name="command">The command containing updated freezer details.</param>
    /// <param name="ct">The cancellation token to observe while awaiting the operation.</param>
    /// <returns>A <see cref="FreezerDto"/> representing the updated freezer.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the freezer with the specified ID is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when the updated freezer name is null or whitespace.</exception>
    public async Task<FreezerDto> HandleAsync(UpdateFreezerCommand command, CancellationToken ct = default)
    {
        var freezer = await _repository.GetByIdAsync(FreezerId.From(command.Id), ct);
        if (freezer is null)
        {
            throw new KeyNotFoundException($"Freezer with ID '{command.Id}' was not found.");
        }

        freezer.UpdateDetails(command.Name, command.Location, command.DisplayOrder);
        await _repository.UpdateAsync(freezer, ct);
        return FreezerDto.FromDomain(freezer);
    }
}
