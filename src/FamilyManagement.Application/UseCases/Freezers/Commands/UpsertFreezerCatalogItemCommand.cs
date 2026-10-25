using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

public record UpsertFreezerCatalogItemCommand(
    string Name,
    FreezerCategory Category,
    FreezerUnit Unit,
    decimal? MinimumQuantity = null);

public class UpsertFreezerCatalogItemCommandHandler
{
    private readonly IFreezerCatalogRepository _repository;

    public UpsertFreezerCatalogItemCommandHandler(IFreezerCatalogRepository repository)
    {
        _repository = repository;
    }

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
