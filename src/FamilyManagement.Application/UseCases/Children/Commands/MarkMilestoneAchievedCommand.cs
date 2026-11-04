using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Commands;

/// <summary>
/// Command to mark an existing milestone as achieved.
/// </summary>
/// <param name="Id">The unique identifier of the milestone.</param>
/// <param name="AchievedDate">The date when the milestone was achieved.</param>
/// <param name="Notes">Optional notes regarding the achievement.</param>
/// <param name="PhotoUrl">An optional URL to a photo documenting the milestone.</param>
public record MarkMilestoneAchievedCommand(
    Guid Id,
    DateTime AchievedDate,
    string? Notes = null,
    string? PhotoUrl = null);

/// <summary>
/// Handler for marking a child milestone as achieved.
/// </summary>
public class MarkMilestoneAchievedCommandHandler
{
    private readonly IChildMilestoneRepository _repository;
    private readonly IChildProfileRepository _childRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkMilestoneAchievedCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The child milestone repository.</param>
    /// <param name="childRepository">The child profile repository.</param>
    public MarkMilestoneAchievedCommandHandler(
        IChildMilestoneRepository repository,
        IChildProfileRepository childRepository)
    {
        _repository = repository;
        _childRepository = childRepository;
    }

    /// <summary>
    /// Handles marking a milestone as achieved, calculates the child's age at achievement, and updates the repository.
    /// </summary>
    /// <param name="command">The command containing milestone achievement details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the updated <see cref="ChildMilestoneDto"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the milestone with the specified ID is not found.</exception>
    public async Task<ChildMilestoneDto> HandleAsync(
        MarkMilestoneAchievedCommand command,
        CancellationToken cancellationToken = default)
    {
        var milestone = await _repository.GetByIdAsync(MilestoneId.From(command.Id), cancellationToken);
        if (milestone is null)
        {
            throw new KeyNotFoundException($"Milestone with ID '{command.Id}' was not found.");
        }

        var child = await _childRepository.GetByIdAsync(milestone.ChildId, cancellationToken);
        var ageAtAchieved = child != null ? child.GetAge(command.AchievedDate).TotalMonths : 0;

        milestone.MarkAchieved(command.AchievedDate, ageAtAchieved, command.Notes, command.PhotoUrl);
        await _repository.UpdateAsync(milestone, cancellationToken);

        return ChildMilestoneDto.FromDomain(milestone);
    }
}
