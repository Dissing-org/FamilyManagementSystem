using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Queries;

public record GetGrowthHistoryQuery(Guid ChildId);

public class GetGrowthHistoryQueryHandler
{
    private readonly IGrowthMeasurementRepository _repository;

    public GetGrowthHistoryQueryHandler(IGrowthMeasurementRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<GrowthMeasurementDto>> HandleAsync(
        GetGrowthHistoryQuery query,
        CancellationToken cancellationToken = default)
    {
        var measurements = await _repository.GetByChildIdAsync(ChildId.From(query.ChildId), cancellationToken);
        return measurements.Select(GrowthMeasurementDto.FromDomain).ToList();
    }
}
