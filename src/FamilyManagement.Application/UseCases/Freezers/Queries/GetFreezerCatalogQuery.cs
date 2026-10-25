using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Freezers.Queries;

public record GetFreezerCatalogQuery();

public class GetFreezerCatalogQueryHandler
{
    private readonly IFreezerCatalogRepository _repository;

    public GetFreezerCatalogQueryHandler(IFreezerCatalogRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<FreezerCatalogItemDto>> HandleAsync(GetFreezerCatalogQuery query, CancellationToken ct = default)
    {
        var items = await _repository.GetAllAsync(ct);
        return items
            .OrderBy(i => i.Name, StringComparer.OrdinalIgnoreCase)
            .Select(FreezerCatalogItemDto.FromDomain)
            .ToList();
    }
}
