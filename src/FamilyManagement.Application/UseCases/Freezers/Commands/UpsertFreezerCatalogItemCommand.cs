using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

/// <summary>
/// Command to create or update a freezer catalog template item.
/// </summary>
/// <param name="Name">The name or label of the catalog item.</param>
/// <param name="Category">The default category for items of this template.</param>
/// <param name="Unit">The default measurement unit for items of this template.</param>
/// <param name="MinimumQuantity">The optional default minimum quantity threshold.</param>
public record UpsertFreezerCatalogItemCommand(
    string Name,
    FreezerCategory Category,
    FreezerUnit Unit,
    decimal? MinimumQuantity = null);

/// <summary>
/// Handler for processing <see cref="UpsertFreezerCatalogItemCommand"/> requests.
/// </summary>
public class UpsertFreezerCatalogItemCommandHandler
{
    private readonly IFreezerCatalogRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpsertFreezerCatalogItemCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository for accessing and modifying freezer catalog items.</param>
    public UpsertFreezerCatalogItemCommandHandler(IFreezerCatalogRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles upserting a freezer catalog template item by name.
    /// </summary>
    /// <param name="command">The command containing catalog item template specifications.</param>
    /// <param name="ct">The cancellation token to observe while awaiting the operation.</param>
    /// <returns>A <see cref="FreezerCatalogItemDto"/> representing the created or updated catalog item.</returns>
    /// <exception cref="ArgumentException">Thrown when the item name is null, whitespace, or exceeds 150 characters, or when minimum quantity is negative.</exception>
    public async Task<FreezerCatalogItemDto> HandleAsync(UpsertFreezerCatalogItemCommand command, CancellationToken ct = default)
    {
        var trimmedName = command.Name?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(trimmedName))
        {
            throw new ArgumentException("Catalog item name is required.", nameof(command));
        }

        var existing = await _repository.GetByNameAsync(trimmedName, ct);
        if (existing is not null)
        {
            existing.UpdateDefaults(command.Category, command.Unit, command.MinimumQuantity);
            await _repository.UpdateAsync(existing, ct);
            return FreezerCatalogItemDto.FromDomain(existing);
        }

        var newItem = FreezerCatalogItem.Create(
            trimmedName,
            command.Category,
            command.Unit,
            command.MinimumQuantity);

        try
        {
            var saved = await _repository.AddAsync(newItem, ct);
            return FreezerCatalogItemDto.FromDomain(saved);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            var concurrentExisting = await _repository.GetByNameAsync(trimmedName, ct);
            if (concurrentExisting is not null)
            {
                concurrentExisting.UpdateDefaults(command.Category, command.Unit, command.MinimumQuantity);
                await _repository.UpdateAsync(concurrentExisting, ct);
                return FreezerCatalogItemDto.FromDomain(concurrentExisting);
            }

            throw;
        }
    }
}
