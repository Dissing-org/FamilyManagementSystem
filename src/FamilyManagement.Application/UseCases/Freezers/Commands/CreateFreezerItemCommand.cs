using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FamilyManagement.Application.UseCases.Freezers.Commands;

/// <summary>
/// Command to create and store a new item in a freezer compartment.
/// </summary>
/// <param name="CompartmentId">The unique identifier of the target compartment.</param>
/// <param name="Name">The name or description of the item.</param>
/// <param name="Category">The category of the item.</param>
/// <param name="Quantity">The quantity of the item.</param>
/// <param name="Unit">The measurement unit of the item.</param>
/// <param name="FreezeDate">The date when the item was frozen.</param>
/// <param name="ExpiryDate">The optional expiration date of the item.</param>
/// <param name="MinimumQuantity">The optional minimum stock quantity threshold.</param>
/// <param name="Notes">Optional notes or remarks.</param>
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

/// <summary>
/// Handler for processing <see cref="CreateFreezerItemCommand"/> requests.
/// </summary>
public class CreateFreezerItemCommandHandler
{
    private readonly IFreezerItemRepository _repository;
    private readonly IFreezerCatalogRepository? _catalogRepository;
    private readonly ILogger<CreateFreezerItemCommandHandler>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateFreezerItemCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository for accessing and modifying freezer item inventory.</param>
    /// <param name="catalogRepository">The optional repository for catalog item templates, used for auto-syncing.</param>
    /// <param name="logger">The optional logger for logging synchronization warnings.</param>
    public CreateFreezerItemCommandHandler(
        IFreezerItemRepository repository,
        IFreezerCatalogRepository? catalogRepository = null,
        ILogger<CreateFreezerItemCommandHandler>? logger = null)
    {
        _repository = repository;
        _catalogRepository = catalogRepository;
        _logger = logger;
    }

    /// <summary>
    /// Handles creating a new freezer item and optionally synchronizing it with the catalog templates.
    /// </summary>
    /// <param name="command">The command containing new freezer item details.</param>
    /// <param name="ct">The cancellation token to observe while awaiting the operation.</param>
    /// <returns>A <see cref="FreezerItemDto"/> representing the newly created freezer item.</returns>
    /// <exception cref="ArgumentException">Thrown when the item name is null or whitespace, or when quantity or minimum quantity is negative.</exception>
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
