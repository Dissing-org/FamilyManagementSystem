using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Queries;

/// <summary>
/// Query to retrieve the growth measurement history for a child.
/// </summary>
/// <param name="ChildId">The unique identifier of the child.</param>
public record GetGrowthHistoryQuery(Guid ChildId);

/// <summary>
/// Handler for retrieving the growth measurement history for a child.
/// </summary>
public class GetGrowthHistoryQueryHandler
{
    private readonly IGrowthMeasurementRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetGrowthHistoryQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The growth measurement repository.</param>
    public GetGrowthHistoryQueryHandler(IGrowthMeasurementRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles retrieving the growth measurements recorded for a child.
    /// </summary>
    /// <param name="query">The query containing the child identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a list of <see cref="GrowthMeasurementDto"/> instances.</returns>
    public async Task<List<GrowthMeasurementDto>> HandleAsync(
        GetGrowthHistoryQuery query,
        CancellationToken cancellationToken = default)
    {
        var measurements = await _repository.GetByChildIdAsync(ChildId.From(query.ChildId), cancellationToken);
        return measurements.Select(GrowthMeasurementDto.FromDomain).ToList();
    }
}
