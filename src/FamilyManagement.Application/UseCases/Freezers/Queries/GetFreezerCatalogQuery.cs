using System.Globalization;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Freezers.Queries;

public record GetFreezerCatalogQuery();

public class GetFreezerCatalogQueryHandler
{
    private static readonly StringComparer DanishComparer =
        StringComparer.Create(new CultureInfo("da-DK"), ignoreCase: true);

    private readonly IFreezerCatalogRepository _repository;

    public GetFreezerCatalogQueryHandler(IFreezerCatalogRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<FreezerCatalogItemDto>> HandleAsync(GetFreezerCatalogQuery query, CancellationToken ct = default)
    {
        var items = await _repository.GetAllAsync(ct);
        return items
            .OrderBy(i => i.Name, DanishComparer)
            .Select(FreezerCatalogItemDto.FromDomain)
            .ToList();
    }
}
