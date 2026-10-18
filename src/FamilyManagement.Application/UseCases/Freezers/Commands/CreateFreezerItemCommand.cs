using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

public record CreateFreezerItemCommand(
    Guid CompartmentId,
    string Name,
    FreezerCategory Category,
    decimal Quantity,
    FreezerUnit Unit,
    DateTime FreezeDate,
    DateTime? ExpiryDate = null,
    decimal? MinimumQuantity = null,
    string? Notes = null);

public class CreateFreezerItemCommandHandler
{
    private readonly IFreezerItemRepository _repository;

    public CreateFreezerItemCommandHandler(IFreezerItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<FreezerItemDto> HandleAsync(CreateFreezerItemCommand command, CancellationToken ct = default)
    {
        var item = FreezerItem.Create(
            command.Name,
            FreezerCompartmentId.From(command.CompartmentId),
            command.Category,
            command.Quantity,
            command.Unit,
            command.FreezeDate,
            command.ExpiryDate,
            command.MinimumQuantity,
            command.Notes);

        await _repository.AddAsync(item, ct);
        return FreezerItemDto.FromDomain(item);
    }
}
