using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

public record DeleteFreezerCatalogItemCommand(Guid Id);

public class DeleteFreezerCatalogItemCommandHandler
{
    private readonly IFreezerCatalogRepository _repository;

    public DeleteFreezerCatalogItemCommandHandler(IFreezerCatalogRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(DeleteFreezerCatalogItemCommand command, CancellationToken ct = default)
    {
        await _repository.DeleteAsync(FreezerCatalogItemId.From(command.Id), ct);
        return true;
    }
}
