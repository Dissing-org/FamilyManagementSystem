using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

public record ConsumeFreezerItemCommand(
    Guid ItemId,
    decimal QuantityToConsume,
    ConsumptionReason Reason,
    string? Notes = null);

public class ConsumeFreezerItemCommandHandler
{
    private readonly IFreezerItemRepository _repository;

    public ConsumeFreezerItemCommandHandler(IFreezerItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<FreezerItemDto> HandleAsync(ConsumeFreezerItemCommand command, CancellationToken ct = default)
    {
        var item = await _repository.GetByIdAsync(FreezerItemId.From(command.ItemId), ct);
        if (item is null)
        {
            throw new KeyNotFoundException($"Freezer item with ID '{command.ItemId}' was not found.");
        }

        var transaction = item.Consume(command.QuantityToConsume, command.Reason, command.Notes);
        await _repository.AddTransactionAsync(transaction, ct);
        await _repository.UpdateAsync(item, ct);

        return FreezerItemDto.FromDomain(item);
    }
}
