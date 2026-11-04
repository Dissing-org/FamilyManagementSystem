using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

/// <summary>
/// Command to delete a freezer unit by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the freezer to delete.</param>
public record DeleteFreezerCommand(Guid Id);

/// <summary>
/// Handler for processing <see cref="DeleteFreezerCommand"/> requests.
/// </summary>
public class DeleteFreezerCommandHandler
{
    private readonly IFreezerRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteFreezerCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository for accessing and deleting freezer aggregate data.</param>
    public DeleteFreezerCommandHandler(IFreezerRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles deleting a freezer unit by its identifier.
    /// </summary>
    /// <param name="command">The command containing the freezer identifier to delete.</param>
    /// <param name="ct">The cancellation token to observe while awaiting the operation.</param>
    /// <returns><c>true</c> if the freezer was found and deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> HandleAsync(DeleteFreezerCommand command, CancellationToken ct = default)
    {
        var freezerId = FreezerId.From(command.Id);
        var freezer = await _repository.GetByIdAsync(freezerId, ct);
        if (freezer is null)
        {
            return false;
        }

        await _repository.DeleteAsync(freezerId, ct);
        return true;
    }
}
