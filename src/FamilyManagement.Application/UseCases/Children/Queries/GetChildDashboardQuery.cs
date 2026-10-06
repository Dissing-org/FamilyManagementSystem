using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Queries;

public record GetChildDashboardQuery(Guid ChildId);

public class GetChildDashboardQueryHandler
{
    private readonly IChildProfileRepository _childRepository;
    private readonly IGrowthMeasurementRepository _growthRepository;
    private readonly IChildMilestoneRepository _milestoneRepository;

    public GetChildDashboardQueryHandler(
        IChildProfileRepository childRepository,
        IGrowthMeasurementRepository growthRepository,
        IChildMilestoneRepository milestoneRepository)
    {
        _childRepository = childRepository;
        _growthRepository = growthRepository;
        _milestoneRepository = milestoneRepository;
    }

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

        var allMilestones = await _milestoneRepository.GetByChildIdAsync(child.Id, cancellationToken: cancellationToken);

        var achieved = allMilestones
            .Where(m => m.Status == MilestoneStatus.Achieved)
            .OrderByDescending(m => m.AchievedDate ?? m.CreatedAt)
            .Select(ChildMilestoneDto.FromDomain)
            .ToList();

        var childAgeMonths = child.GetAge().TotalMonths;

        var upcoming = allMilestones
            .Where(m => m.Status == MilestoneStatus.Expected)
            .OrderBy(m => m.ExpectedAgeMonths)
            .Select(ChildMilestoneDto.FromDomain)
            .Take(5)
            .ToList();

        return new ChildDashboardDto(
            ChildProfileDto.FromDomain(child),
            latestMeasurement != null ? GrowthMeasurementDto.FromDomain(latestMeasurement) : null,
            measurements.Count,
            achieved.Count,
            allMilestones.Count(m => m.Status == MilestoneStatus.Expected),
            achieved.Take(5).ToList(),
            upcoming);
    }
}
