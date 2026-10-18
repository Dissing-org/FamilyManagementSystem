namespace FamilyManagement.Application.DTOs.Freezers;

public record FreezerDashboardDto(
    IReadOnlyList<FreezerDto> Freezers,
    int TotalItemCount,
    int ExpiringSoonCount,
    int ExpiredCount,
    int LowStockCount,
    IReadOnlyList<FreezerItemDto> UrgentItems,
    IReadOnlyDictionary<string, int> ItemsByCategory);
