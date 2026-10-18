using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

public record UpdateFreezerCommand(
    Guid Id,
    string Name,
    string? Location = null,
    int DisplayOrder = 0);

public class UpdateFreezerCommandHandler
{
    private readonly IFreezerRepository _repository;

    public UpdateFreezerCommandHandler(IFreezerRepository repository)
    {
        _repository = repository;
    }

    public async Task<FreezerDto> HandleAsync(UpdateFreezerCommand command, CancellationToken ct = default)
    {
        var freezer = await _repository.GetByIdAsync(FreezerId.From(command.Id), ct);
        if (freezer is null)
        {
            throw new KeyNotFoundException($"Freezer with ID '{command.Id}' was not found.");
        }

        freezer.UpdateDetails(command.Name, command.Location, command.DisplayOrder);
        await _repository.UpdateAsync(freezer, ct);
        return FreezerDto.FromDomain(freezer);
    }
}
