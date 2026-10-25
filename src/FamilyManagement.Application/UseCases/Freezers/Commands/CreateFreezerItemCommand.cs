using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

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
    private readonly IFreezerCatalogRepository? _catalogRepository;
    private readonly ILogger<CreateFreezerItemCommandHandler>? _logger;

    public CreateFreezerItemCommandHandler(
        IFreezerItemRepository repository,
        IFreezerCatalogRepository? catalogRepository = null,
        ILogger<CreateFreezerItemCommandHandler>? logger = null)
    {
        _repository = repository;
        _catalogRepository = catalogRepository;
        _logger = logger;
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

        if (_catalogRepository is not null)
        {
            try
            {
                var existing = await _catalogRepository.GetByNameAsync(item.Name, ct);
                if (existing is not null)
                {
                    existing.UpdateDefaults(item.Category, item.Unit, item.MinimumQuantity);
                    await _catalogRepository.UpdateAsync(existing, ct);
                }
                else
                {
                    var newCatalogItem = FreezerCatalogItem.Create(item.Name, item.Category, item.Unit, item.MinimumQuantity);
                    await _catalogRepository.AddAsync(newCatalogItem, ct);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger?.LogWarning(ex, "Failed to auto-sync catalog item for '{Name}'.", item.Name);
            }
        }

        return FreezerItemDto.FromDomain(item);
    }
}
