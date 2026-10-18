using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

public record UpdateFreezerItemCommand(
    Guid Id,
    Guid CompartmentId,
    string Name,
    FreezerCategory Category,
    decimal Quantity,
    FreezerUnit Unit,
    DateTime FreezeDate,
    DateTime? ExpiryDate,
    decimal? MinimumQuantity,
    string? Notes);

public class UpdateFreezerItemCommandHandler
{
    private readonly IFreezerItemRepository _repository;

    public UpdateFreezerItemCommandHandler(IFreezerItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<FreezerItemDto> HandleAsync(UpdateFreezerItemCommand command, CancellationToken ct = default)
    {
        var item = await _repository.GetByIdAsync(FreezerItemId.From(command.Id), ct);
        if (item is null)
        {
            throw new KeyNotFoundException($"Freezer item with ID '{command.Id}' was not found.");
        }

        item.UpdateDetails(
            command.Name,
            FreezerCompartmentId.From(command.CompartmentId),
            command.Category,
            command.Quantity,
            command.Unit,
            command.FreezeDate,
            command.ExpiryDate,
            command.MinimumQuantity,
            command.Notes);

        await _repository.UpdateAsync(item, ct);
        return FreezerItemDto.FromDomain(item);
    }
}
