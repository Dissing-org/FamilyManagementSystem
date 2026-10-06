using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Queries;

public record GetMilestonesQuery(
    Guid ChildId,
    MilestoneStatus? Status = null,
    MilestoneCategory? Category = null);

public class GetMilestonesQueryHandler
{
    private readonly IChildMilestoneRepository _repository;

    public GetMilestonesQueryHandler(IChildMilestoneRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ChildMilestoneDto>> HandleAsync(
        GetMilestonesQuery query,
        CancellationToken cancellationToken = default)
    {
        var milestones = await _repository.GetByChildIdAsync(
            ChildId.From(query.ChildId),
            query.Status,
            query.Category,
            cancellationToken);

        return milestones.Select(ChildMilestoneDto.FromDomain).ToList();
    }
}
