using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Commands;

/// <summary>
/// Command to create a milestone for a child, either as achieved or expected.
/// </summary>
/// <param name="ChildId">The unique identifier of the child.</param>
/// <param name="Title">The title or name of the milestone.</param>
/// <param name="Category">The developmental category of the milestone.</param>
/// <param name="IsAchieved">Indicates whether the milestone has already been achieved.</param>
/// <param name="ExpectedAgeMonths">The expected age in months when the milestone is typically reached.</param>
/// <param name="ExpectedWindowMaxMonths">The upper bound of the expected developmental window in months.</param>
/// <param name="AchievedDate">The optional date when the milestone was achieved.</param>
/// <param name="Description">An optional description of the milestone.</param>
/// <param name="Notes">Optional notes regarding the achievement.</param>
/// <param name="PhotoUrl">An optional URL to a photo documenting the milestone.</param>
public record CreateMilestoneCommand(
    Guid ChildId,
    string Title,
    MilestoneCategory Category,
    bool IsAchieved,
    int? ExpectedAgeMonths = null,
    int? ExpectedWindowMaxMonths = null,
    DateTime? AchievedDate = null,
    string? Description = null,
    string? Notes = null,
    string? PhotoUrl = null);

/// <summary>
/// Handler for creating a new child milestone.
/// </summary>
public class CreateMilestoneCommandHandler
{
    private readonly IChildMilestoneRepository _repository;
    private readonly IChildProfileRepository _childRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateMilestoneCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The child milestone repository.</param>
    /// <param name="childRepository">The child profile repository.</param>
    public CreateMilestoneCommandHandler(
        IChildMilestoneRepository repository,
        IChildProfileRepository childRepository)
    {
        _repository = repository;
        _childRepository = childRepository;
    }

    /// <summary>
    /// Handles the creation of a milestone for a child and persists it.
    /// </summary>
    /// <param name="command">The command containing the milestone creation details.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the created <see cref="ChildMilestoneDto"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the child with the specified ID is not found.</exception>
    public async Task<ChildMilestoneDto> HandleAsync(
        CreateMilestoneCommand command,
        CancellationToken cancellationToken = default)
    {
        var child = await _childRepository.GetByIdAsync(ChildId.From(command.ChildId), cancellationToken);
        if (child is null)
        {
            throw new KeyNotFoundException($"Child with ID '{command.ChildId}' was not found.");
        }

        ChildMilestone milestone;
        if (command.IsAchieved)
        {
            var achievedDate = command.AchievedDate ?? DateTime.UtcNow;
            var ageAtAchieved = child.GetAge(achievedDate).TotalMonths;
            milestone = ChildMilestone.CreateAchieved(
                child.Id,
                command.Title,
                command.Category,
                achievedDate,
                ageAtAchieved,
                command.Notes,
                command.PhotoUrl);
        }
        else
        {
            milestone = ChildMilestone.CreateExpected(
                child.Id,
                command.Title,
                command.Category,
                command.ExpectedAgeMonths ?? 12,
                command.ExpectedWindowMaxMonths,
                command.Description,
                isStandardGuideline: false);
        }

        await _repository.AddAsync(milestone, cancellationToken);
        return ChildMilestoneDto.FromDomain(milestone);
    }
}
