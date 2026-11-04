using System.Globalization;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Freezers.Queries;

/// <summary>
/// Query to retrieve all items from the freezer item template catalog.
/// </summary>
public record GetFreezerCatalogQuery();

/// <summary>
/// Handler for processing <see cref="GetFreezerCatalogQuery"/> requests.
/// </summary>
public class GetFreezerCatalogQueryHandler
{
    private static readonly StringComparer DanishComparer =
        StringComparer.Create(new CultureInfo("da-DK"), ignoreCase: true);

    private readonly IFreezerCatalogRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetFreezerCatalogQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The repository for accessing freezer catalog items.</param>
    public GetFreezerCatalogQueryHandler(IFreezerCatalogRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles retrieving all freezer catalog items, sorted alphabetically according to Danish language conventions.
    /// </summary>
    /// <param name="query">The query requesting all catalog items.</param>
    /// <param name="ct">The cancellation token to observe while awaiting the operation.</param>
    /// <returns>A read-only list of <see cref="FreezerCatalogItemDto"/> representing all catalog templates.</returns>
    public async Task<IReadOnlyList<FreezerCatalogItemDto>> HandleAsync(GetFreezerCatalogQuery query, CancellationToken ct = default)
    {
        var items = await _repository.GetAllAsync(ct);
        return items
            .OrderBy(i => i.Name, DanishComparer)
            .Select(FreezerCatalogItemDto.FromDomain)
            .ToList();
    }
}
