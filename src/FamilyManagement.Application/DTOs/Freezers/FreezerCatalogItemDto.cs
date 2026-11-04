using System.Text.Json.Serialization;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Application.DTOs.Freezers;

/// <summary>
/// Data transfer object representing a freezer catalog item used as a template for frequently added items.
/// </summary>
/// <param name="Id">The unique identifier of the catalog item.</param>
/// <param name="Name">The name of the catalog item.</param>
/// <param name="DefaultCategory">The default category assigned to items based on this template.</param>
/// <param name="DefaultUnit">The default measurement unit assigned to items based on this template.</param>
/// <param name="DefaultMinimumQuantity">The optional default minimum quantity threshold.</param>
/// <param name="LastUsedAt">The timestamp when this catalog template was last used.</param>
public record FreezerCatalogItemDto(
    Guid Id,
    string Name,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] FreezerCategory DefaultCategory,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] FreezerUnit DefaultUnit,
    decimal? DefaultMinimumQuantity,
    DateTime LastUsedAt)
{
    /// <summary>
    /// Creates a new <see cref="FreezerCatalogItemDto"/> from a domain <see cref="FreezerCatalogItem"/> entity.
    /// </summary>
    /// <param name="item">The domain freezer catalog item entity.</param>
    /// <returns>A <see cref="FreezerCatalogItemDto"/> populated with values from the domain entity.</returns>
    public static FreezerCatalogItemDto FromDomain(FreezerCatalogItem item) =>
        new(
            item.Id.Value,
            item.Name,
            item.DefaultCategory,
            item.DefaultUnit,
            item.DefaultMinimumQuantity,
            item.LastUsedAt);
}
