using FamilyManagement.Domain.Entities;

namespace FamilyManagement.Application.DTOs.Freezers;

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
