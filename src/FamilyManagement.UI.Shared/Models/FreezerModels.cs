using System.Text.Json.Serialization;

namespace FamilyManagement.UI.Shared.Models;

public record FreezerCompartmentDto(
    Guid Id,
    Guid FreezerId,
    string Name,
    int DisplayOrder,
    int ItemCount);

public record FreezerDto(
    Guid Id,
    string Name,
    string? Location,
    int DisplayOrder,
    DateTime CreatedAt,
    IReadOnlyList<FreezerCompartmentDto> Compartments);

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
    DateTime? UpdatedAt);

public record FreezerTransactionDto(
    Guid Id,
    Guid ItemId,
    decimal QuantityChanged,
    decimal RemainingQuantity,
    string Reason,
    string? Notes,
    DateTime Timestamp);

public record FreezerDashboardDto(
    IReadOnlyList<FreezerDto> Freezers,
    int TotalItemCount,
    int ExpiringSoonCount,
    int ExpiredCount,
    int LowStockCount,
    IReadOnlyList<FreezerItemDto> UrgentItems,
    IReadOnlyDictionary<string, int> ItemsByCategory);

public class CreateFreezerRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

public class UpdateFreezerRequest
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Location { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

public class CreateCompartmentRequest
{
    public Guid FreezerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int DisplayOrder { get; set; } = 0;
}

public class CreateFreezerItemRequest
{
    public Guid CompartmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = "MeatAndPoultry";
    public decimal Quantity { get; set; } = 1;
    public string Unit { get; set; } = "Pieces";
    public DateTime FreezeDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    public decimal? MinimumQuantity { get; set; }
    public string? Notes { get; set; }
}

public class UpdateFreezerItemRequest
{
    public Guid Id { get; set; }
    public Guid CompartmentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = "MeatAndPoultry";
    public decimal Quantity { get; set; } = 1;
    public string Unit { get; set; } = "Pieces";
    public DateTime FreezeDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiryDate { get; set; }
    public decimal? MinimumQuantity { get; set; }
    public string? Notes { get; set; }
}

public class ConsumeItemRequest
{
    public Guid Id { get; set; }
    public decimal QuantityToConsume { get; set; } = 1;
    public string Reason { get; set; } = "Consumed";
    public string? Notes { get; set; }
}
