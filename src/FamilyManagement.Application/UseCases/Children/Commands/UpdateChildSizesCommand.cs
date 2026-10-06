using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Commands;

public record UpdateChildSizesCommand(
    Guid ChildId,
    string? ClothesSize,
    string? ShoeSize,
    string? HatSize,
    string? DiaperSize);

public class UpdateChildSizesCommandHandler
{
    private readonly IChildProfileRepository _repository;

    public UpdateChildSizesCommandHandler(IChildProfileRepository repository)
    {
        _repository = repository;
    }

    public async Task<ChildProfileDto> HandleAsync(
        UpdateChildSizesCommand command,
        CancellationToken cancellationToken = default)
    {
        var child = await _repository.GetByIdAsync(ChildId.From(command.ChildId), cancellationToken);
        if (child is null)
        {
            throw new KeyNotFoundException($"Child with ID '{command.ChildId}' was not found.");
        }

        var sizes = new ChildWardrobeSizes(
            command.ClothesSize,
            command.ShoeSize,
            command.HatSize,
            command.DiaperSize);

        child.UpdateSizes(sizes);
        await _repository.UpdateAsync(child, cancellationToken);

        return ChildProfileDto.FromDomain(child);
    }
}
