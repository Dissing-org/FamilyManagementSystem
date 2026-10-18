using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

public record DeleteFreezerItemCommand(Guid Id);

public class DeleteFreezerItemCommandHandler
{
    private readonly IFreezerItemRepository _repository;

    public DeleteFreezerItemCommandHandler(IFreezerItemRepository repository)
    {
        _repository = repository;
    }

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
