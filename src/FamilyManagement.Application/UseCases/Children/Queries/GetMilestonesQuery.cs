using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Queries;

/// <summary>
/// Query to retrieve milestones for a child, optionally filtered by status and category.
/// </summary>
/// <param name="ChildId">The unique identifier of the child.</param>
/// <param name="Status">The optional status filter.</param>
/// <param name="Category">The optional category filter.</param>
public record GetMilestonesQuery(
    Guid ChildId,
    MilestoneStatus? Status = null,
    MilestoneCategory? Category = null);

/// <summary>
/// Handler for retrieving developmental milestones for a child.
/// </summary>
public class GetMilestonesQueryHandler
{
    private readonly IChildMilestoneRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMilestonesQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The child milestone repository.</param>
    public GetMilestonesQueryHandler(IChildMilestoneRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles retrieving developmental milestones for a child based on optional filtering criteria.
    /// </summary>
    /// <param name="query">The query containing the child identifier and optional filters.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a list of matching <see cref="ChildMilestoneDto"/> instances.</returns>
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
