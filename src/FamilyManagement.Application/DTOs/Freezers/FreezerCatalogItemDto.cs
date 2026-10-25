using System.Text.Json.Serialization;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Application.DTOs.Freezers;

public record FreezerCatalogItemDto(
    Guid Id,
    string Name,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] FreezerCategory DefaultCategory,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] FreezerUnit DefaultUnit,
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
