using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Application.DTOs.Freezers;

public record FreezerCatalogItemDto(
    Guid Id,
    string Name,
    FreezerCategory DefaultCategory,
    FreezerUnit DefaultUnit,
    decimal? DefaultMinimumQuantity,
    DateTime LastUsedAt)
{
    public static FreezerCatalogItemDto FromDomain(FreezerCatalogItem item) =>
        new(
            item.Id.Value,
            item.Name,
            item.DefaultCategory,
            item.DefaultUnit,
            item.DefaultMinimumQuantity,
            item.LastUsedAt);
}
