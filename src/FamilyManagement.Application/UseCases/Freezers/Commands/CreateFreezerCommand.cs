using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

/// <summary>
/// Command to create a new freezer unit.
/// </summary>
/// <param name="Name">The name or label of the new freezer unit.</param>
/// <param name="Location">The optional physical location of the freezer.</param>
/// <param name="DisplayOrder">The display ordering index for the freezer.</param>
public record CreateFreezerCommand(
    string Name,
    string? Location = null,
    int DisplayOrder = 0);

/// <summary>
/// Handler for processing <see cref="CreateFreezerCommand"/> requests.
/// </summary>
public class CreateFreezerCommandHandler
{
    private readonly IFreezerRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateFreezerCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository for persisting freezer aggregate data.</param>
    public CreateFreezerCommandHandler(IFreezerRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles creating and saving a new freezer unit.
    /// </summary>
    /// <param name="command">The command containing new freezer details.</param>
    /// <param name="ct">The cancellation token to observe while awaiting the operation.</param>
    /// <returns>A <see cref="FreezerDto"/> representing the newly created freezer.</returns>
    /// <exception cref="ArgumentException">Thrown when the freezer name is null or whitespace.</exception>
    public async Task<FreezerDto> HandleAsync(CreateFreezerCommand command, CancellationToken ct = default)
    {
        var freezer = Freezer.Create(command.Name, command.Location, command.DisplayOrder);
        await _repository.AddAsync(freezer, ct);
        return FreezerDto.FromDomain(freezer);
    }
}
