namespace FamilyManagement.Application.DTOs.Freezers;

public record FreezerAlertsDto(
    IReadOnlyList<FreezerItemDto> ExpiringSoonItems,
    IReadOnlyList<FreezerItemDto> ExpiredItems,
    IReadOnlyList<FreezerItemDto> LowStockItems);
