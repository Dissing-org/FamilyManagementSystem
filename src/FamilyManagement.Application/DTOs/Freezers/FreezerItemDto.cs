using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Application.DTOs.Freezers;

/// <summary>
/// Data transfer object representing an individual item stored in a freezer compartment.
/// </summary>
/// <param name="Id">The unique identifier of the freezer item.</param>
/// <param name="CompartmentId">The unique identifier of the compartment storing this item.</param>
/// <param name="Name">The name or description of the item.</param>
/// <param name="Category">The category of the item as a string.</param>
/// <param name="Quantity">The quantity of the item currently available.</param>
/// <param name="Unit">The unit of measure as a string.</param>
/// <param name="FreezeDate">The date when the item was frozen.</param>
/// <param name="ExpiryDate">The optional expiration date of the item.</param>
/// <param name="MinimumQuantity">The optional minimum stock threshold.</param>
/// <param name="Notes">Optional notes or remarks regarding the item.</param>
/// <param name="FreshnessStatus">The evaluated freshness status of the item as a string.</param>
/// <param name="IsLowStock">A value indicating whether the item is at or below its minimum quantity threshold.</param>
/// <param name="CreatedAt">The timestamp when the item record was created.</param>
/// <param name="UpdatedAt">The optional timestamp when the item record was last updated.</param>
public record FreezerItemDto(
    Guid Id,
    Guid CompartmentId,
    string Name,
    string Category,
    decimal Quantity,
    string Unit,
    DateTime FreezeDate,
    DateTime? ExpiryDate,
    decimal? MinimumQuantity,
    string? Notes,
    string FreshnessStatus,
    bool IsLowStock,
    DateTime CreatedAt,
    DateTime? UpdatedAt)
{
    /// <summary>
    /// Creates a new <see cref="FreezerItemDto"/> from a domain <see cref="FreezerItem"/> entity.
    /// </summary>
    /// <param name="item">The domain freezer item entity.</param>
    /// <param name="referenceDate">The optional reference date against which freshness is evaluated. Defaults to current UTC date and time if <c>null</c>.</param>
    /// <returns>A <see cref="FreezerItemDto"/> representing the freezer item and its evaluated freshness and low stock status.</returns>
    public static FreezerItemDto FromDomain(FreezerItem item, DateTime? referenceDate = null)
    {
        var now = referenceDate ?? DateTime.UtcNow;
        return new FreezerItemDto(
            item.Id.Value,
            item.CompartmentId.Value,
            item.Name,
            item.Category.ToString(),
            item.Quantity,
            item.Unit.ToString(),
            item.FreezeDate,
            item.ExpiryDate,
            item.MinimumQuantity,
            item.Notes,
            item.EvaluateFreshness(now).ToString(),
            item.IsLowStock(),
            item.CreatedAt,
            item.UpdatedAt);
    }
}
