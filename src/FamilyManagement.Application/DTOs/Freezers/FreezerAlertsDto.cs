namespace FamilyManagement.Application.DTOs.Freezers;

/// <summary>
/// Data transfer object representing aggregated alerts for freezer inventory items.
/// </summary>
/// <param name="ExpiringSoonItems">Items nearing their expiration date.</param>
/// <param name="ExpiredItems">Items that have passed their expiration date.</param>
/// <param name="LowStockItems">Items whose current quantity is at or below their configured minimum threshold.</param>
public record FreezerAlertsDto(
    IReadOnlyList<FreezerItemDto> ExpiringSoonItems,
    IReadOnlyList<FreezerItemDto> ExpiredItems,
    IReadOnlyList<FreezerItemDto> LowStockItems);
