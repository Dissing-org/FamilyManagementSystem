using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

/// <summary>
/// Command to delete a freezer catalog template item by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the catalog item to delete.</param>
public record DeleteFreezerCatalogItemCommand(Guid Id);

/// <summary>
/// Handler for processing <see cref="DeleteFreezerCatalogItemCommand"/> requests.
/// </summary>
public class DeleteFreezerCatalogItemCommandHandler
{
    private readonly IFreezerCatalogRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteFreezerCatalogItemCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository for accessing and deleting freezer catalog items.</param>
    public DeleteFreezerCatalogItemCommandHandler(IFreezerCatalogRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles deleting a freezer catalog template item.
    /// </summary>
    /// <param name="command">The command containing the catalog item identifier.</param>
    /// <param name="ct">The cancellation token to observe while awaiting the operation.</param>
    /// <returns><c>true</c> indicating the catalog item deletion was executed.</returns>
    public async Task<bool> HandleAsync(DeleteFreezerCatalogItemCommand command, CancellationToken ct = default)
    {
        await _repository.DeleteAsync(FreezerCatalogItemId.From(command.Id), ct);
        return true;
    }
}
