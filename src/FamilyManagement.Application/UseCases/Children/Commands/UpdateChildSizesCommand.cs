using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Commands;

/// <summary>
/// Command to update wardrobe sizes for a child profile.
/// </summary>
/// <param name="ChildId">The unique identifier of the child.</param>
/// <param name="ClothesSize">The updated clothes size.</param>
/// <param name="ShoeSize">The updated shoe size.</param>
/// <param name="HatSize">The updated hat size.</param>
/// <param name="DiaperSize">The updated diaper size.</param>
public record UpdateChildSizesCommand(
    Guid ChildId,
    string? ClothesSize,
    string? ShoeSize,
    string? HatSize,
    string? DiaperSize);

/// <summary>
/// Handler for updating a child's wardrobe sizes.
/// </summary>
public class UpdateChildSizesCommandHandler
{
    private readonly IChildProfileRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateChildSizesCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The child profile repository.</param>
    public UpdateChildSizesCommandHandler(IChildProfileRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles updating the wardrobe sizes for a child profile.
    /// </summary>
    /// <param name="command">The command containing the updated sizes.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the updated <see cref="ChildProfileDto"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the child with the specified ID is not found.</exception>
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
