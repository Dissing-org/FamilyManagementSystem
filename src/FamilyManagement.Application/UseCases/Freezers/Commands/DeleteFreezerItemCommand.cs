using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

/// <summary>
/// Command to delete a freezer item by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the freezer item to delete.</param>
public record DeleteFreezerItemCommand(Guid Id);

/// <summary>
/// Handler for processing <see cref="DeleteFreezerItemCommand"/> requests.
/// </summary>
public class DeleteFreezerItemCommandHandler
{
    private readonly IFreezerItemRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteFreezerItemCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository for accessing and deleting freezer item inventory.</param>
    public DeleteFreezerItemCommandHandler(IFreezerItemRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles deleting a freezer item by its identifier.
    /// </summary>
    /// <param name="command">The command containing the freezer item identifier to delete.</param>
    /// <param name="ct">The cancellation token to observe while awaiting the operation.</param>
    /// <returns><c>true</c> if the item was found and deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> HandleAsync(DeleteFreezerItemCommand command, CancellationToken ct = default)
    {
        var itemId = FreezerItemId.From(command.Id);
        var item = await _repository.GetByIdAsync(itemId, ct);
        if (item is null)
        {
            return false;
        }

        await _repository.DeleteAsync(itemId, ct);
        return true;
    }
}
