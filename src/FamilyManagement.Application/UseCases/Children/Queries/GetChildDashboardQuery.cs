using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Queries;

/// <summary>
/// Query to retrieve dashboard summary data for a child.
/// </summary>
/// <param name="ChildId">The unique identifier of the child.</param>
public record GetChildDashboardQuery(Guid ChildId);

/// <summary>
/// Handler for retrieving the dashboard summary for a child profile.
/// </summary>
public class GetChildDashboardQueryHandler
{
    private readonly IChildProfileRepository _childRepository;
    private readonly IGrowthMeasurementRepository _growthRepository;
    private readonly IChildMilestoneRepository _milestoneRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetChildDashboardQueryHandler"/> class.
    /// </summary>
    /// <param name="childRepository">The child profile repository.</param>
    /// <param name="growthRepository">The growth measurement repository.</param>
    /// <param name="milestoneRepository">The child milestone repository.</param>
    public GetChildDashboardQueryHandler(
        IChildProfileRepository childRepository,
        IGrowthMeasurementRepository growthRepository,
        IChildMilestoneRepository milestoneRepository)
    {
        _childRepository = childRepository;
        _growthRepository = growthRepository;
        _milestoneRepository = milestoneRepository;
    }

    /// <summary>
    /// Handles retrieving dashboard metrics and recent activity for a child.
    /// </summary>
    /// <param name="query">The query containing the child identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the aggregated <see cref="ChildDashboardDto"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the child with the specified ID is not found.</exception>
    public async Task<ChildDashboardDto> HandleAsync(
        GetChildDashboardQuery query,
        CancellationToken cancellationToken = default)
    {
        var child = await _childRepository.GetByIdAsync(ChildId.From(query.ChildId), cancellationToken);
        if (child is null)
        {
            throw new KeyNotFoundException($"Child with ID '{query.ChildId}' was not found.");
        }

        var measurements = await _growthRepository.GetByChildIdAsync(child.Id, cancellationToken);
        var latestMeasurement = measurements.FirstOrDefault();
        var latestHeight = measurements.FirstOrDefault(m => m.HeightCm.HasValue);
        var latestWeight = measurements.FirstOrDefault(m => m.WeightKg.HasValue);
        var latestHead = measurements.FirstOrDefault(m => m.HeadCircumferenceCm.HasValue);

        var allMilestones = await _milestoneRepository.GetByChildIdAsync(child.Id, cancellationToken: cancellationToken);

        var achieved = allMilestones
            .Where(m => m.Status == MilestoneStatus.Achieved)
            .OrderByDescending(m => m.AchievedDate ?? m.CreatedAt)
            .Select(ChildMilestoneDto.FromDomain)
            .ToList();

        var upcoming = allMilestones
            .Where(m => m.Status == MilestoneStatus.Expected)
            .OrderBy(m => m.ExpectedAgeMonths)
            .Select(ChildMilestoneDto.FromDomain)
            .Take(5)
            .ToList();

        return new ChildDashboardDto(
            ChildProfileDto.FromDomain(child),
            latestMeasurement != null ? GrowthMeasurementDto.FromDomain(latestMeasurement) : null,
            latestHeight != null ? GrowthMeasurementDto.FromDomain(latestHeight) : null,
            latestWeight != null ? GrowthMeasurementDto.FromDomain(latestWeight) : null,
            latestHead != null ? GrowthMeasurementDto.FromDomain(latestHead) : null,
            measurements.Count,
            achieved.Count,
            allMilestones.Count(m => m.Status == MilestoneStatus.Expected),
            achieved.Take(5).ToList(),
            upcoming);
    }
}
