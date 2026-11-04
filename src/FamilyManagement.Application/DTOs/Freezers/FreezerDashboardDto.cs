namespace FamilyManagement.Application.DTOs.Freezers;

/// <summary>
/// Data transfer object representing aggregated dashboard metrics and summary views across all freezers.
/// </summary>
/// <param name="Freezers">The list of configured freezer units with compartments.</param>
/// <param name="TotalItemCount">The total number of items across all freezers.</param>
/// <param name="ExpiringSoonCount">The count of items nearing their expiration date.</param>
/// <param name="ExpiredCount">The count of expired items.</param>
/// <param name="LowStockCount">The count of items with low stock levels.</param>
/// <param name="UrgentItems">A prioritised collection of urgent items requiring attention.</param>
/// <param name="ItemsByCategory">Item counts aggregated by item category.</param>
public record FreezerDashboardDto(
    IReadOnlyList<FreezerDto> Freezers,
    int TotalItemCount,
    int ExpiringSoonCount,
    int ExpiredCount,
    int LowStockCount,
    IReadOnlyList<FreezerItemDto> UrgentItems,
    IReadOnlyDictionary<string, int> ItemsByCategory);
