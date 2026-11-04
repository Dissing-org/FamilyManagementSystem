using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

/// <summary>
/// Repository interface for managing <see cref="ChildMilestone"/> entities.
/// </summary>
public interface IChildMilestoneRepository
{
    /// <summary>
    /// Persists a new child milestone to the repository.
    /// </summary>
    /// <param name="milestone">The milestone entity to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddAsync(ChildMilestone milestone, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists a collection of new child milestones to the repository.
    /// </summary>
    /// <param name="milestones">The collection of milestone entities to add.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task AddRangeAsync(IEnumerable<ChildMilestone> milestones, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a child milestone by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the milestone.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the matching milestone or <c>null</c> if not found.</returns>
    Task<ChildMilestone?> GetByIdAsync(MilestoneId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all milestones for a specific child, optionally filtered by status and category.
    /// </summary>
    /// <param name="childId">The unique identifier of the child.</param>
    /// <param name="status">An optional milestone status filter.</param>
    /// <param name="category">An optional milestone category filter.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the list of matching milestones.</returns>
    Task<List<ChildMilestone>> GetByChildIdAsync(
        ChildId childId,
        MilestoneStatus? status = null,
        MilestoneCategory? category = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing child milestone in the repository.
    /// </summary>
    /// <param name="milestone">The milestone entity with updated values.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateAsync(ChildMilestone milestone, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a child milestone from the repository.
    /// </summary>
    /// <param name="milestone">The milestone entity to remove.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DeleteAsync(ChildMilestone milestone, CancellationToken cancellationToken = default);
}
