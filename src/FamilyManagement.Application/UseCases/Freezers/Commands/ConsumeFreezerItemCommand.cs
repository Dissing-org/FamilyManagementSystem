using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

/// <summary>
/// Command to consume a quantity of a freezer item.
/// </summary>
/// <param name="ItemId">The unique identifier of the item to consume.</param>
/// <param name="QuantityToConsume">The amount of the item to consume; must be greater than zero.</param>
/// <param name="Reason">The reason for consumption.</param>
/// <param name="Notes">Optional notes regarding the consumption event.</param>
public record ConsumeFreezerItemCommand(
    Guid ItemId,
    decimal QuantityToConsume,
    ConsumptionReason Reason,
    string? Notes = null);

/// <summary>
/// Handler for processing <see cref="ConsumeFreezerItemCommand"/> requests.
/// </summary>
public class ConsumeFreezerItemCommandHandler
{
    private readonly IFreezerItemRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsumeFreezerItemCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository for accessing and modifying freezer item inventory.</param>
    public ConsumeFreezerItemCommandHandler(IFreezerItemRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles consuming a specified quantity of a freezer item and logging the transaction.
    /// </summary>
    /// <param name="command">The command containing item consumption details.</param>
    /// <param name="ct">The cancellation token to observe while awaiting the operation.</param>
    /// <returns>A <see cref="FreezerItemDto"/> reflecting the updated item state after consumption.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the freezer item with the specified ID is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when the quantity to consume is less than or equal to zero.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the quantity to consume exceeds the currently available quantity.</exception>
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
