using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

/// <summary>
/// Command to update the details of an existing freezer item.
/// </summary>
/// <param name="Id">The unique identifier of the freezer item to update.</param>
/// <param name="CompartmentId">The unique identifier of the destination compartment.</param>
/// <param name="Name">The updated name or description of the item.</param>
/// <param name="Category">The updated category of the item.</param>
/// <param name="Quantity">The updated quantity of the item.</param>
/// <param name="Unit">The updated measurement unit of the item.</param>
/// <param name="FreezeDate">The updated freeze date.</param>
/// <param name="ExpiryDate">The updated optional expiration date.</param>
/// <param name="MinimumQuantity">The updated optional minimum stock quantity threshold.</param>
/// <param name="Notes">Updated optional notes or remarks.</param>
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

/// <summary>
/// Handler for processing <see cref="UpdateFreezerItemCommand"/> requests.
/// </summary>
public class UpdateFreezerItemCommandHandler
{
    private readonly IFreezerItemRepository _repository;
    private readonly IFreezerCatalogRepository? _catalogRepository;
    private readonly ILogger<UpdateFreezerItemCommandHandler>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateFreezerItemCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository for accessing and modifying freezer item inventory.</param>
    /// <param name="catalogRepository">The optional repository for catalog item templates, used for auto-syncing.</param>
    /// <param name="logger">The optional logger for logging synchronization warnings.</param>
    public UpdateFreezerItemCommandHandler(
        IFreezerItemRepository repository,
        IFreezerCatalogRepository? catalogRepository = null,
        ILogger<UpdateFreezerItemCommandHandler>? logger = null)
    {
        _repository = repository;
        _catalogRepository = catalogRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles updating the details of an existing freezer item and synchronizing with the catalog template.
    /// </summary>
    /// <param name="command">The command containing updated freezer item details.</param>
    /// <param name="ct">The cancellation token to observe while awaiting the operation.</param>
    /// <returns>A <see cref="FreezerItemDto"/> representing the updated freezer item.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the freezer item with the specified ID is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when the item name is null or whitespace, or when quantity or minimum quantity is negative.</exception>
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
