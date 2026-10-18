using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

public record DeleteFreezerCommand(Guid Id);

public class DeleteFreezerCommandHandler
{
    private readonly IFreezerRepository _repository;

    public DeleteFreezerCommandHandler(IFreezerRepository repository)
    {
        _repository = repository;
    }

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
