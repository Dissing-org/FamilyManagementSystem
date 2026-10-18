using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

public record CreateFreezerCommand(
    string Name,
    string? Location = null,
    int DisplayOrder = 0);

public class CreateFreezerCommandHandler
{
    private readonly IFreezerRepository _repository;

    public CreateFreezerCommandHandler(IFreezerRepository repository)
    {
        _repository = repository;
    }

    public async Task<FreezerDto> HandleAsync(CreateFreezerCommand command, CancellationToken ct = default)
    {
        var freezer = Freezer.Create(command.Name, command.Location, command.DisplayOrder);
        await _repository.AddAsync(freezer, ct);
        return FreezerDto.FromDomain(freezer);
    }
}
