using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Commands;

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

public class CreateMilestoneCommandHandler
{
    private readonly IChildMilestoneRepository _repository;
    private readonly IChildProfileRepository _childRepository;

    public CreateMilestoneCommandHandler(
        IChildMilestoneRepository repository,
        IChildProfileRepository childRepository)
    {
        _repository = repository;
        _childRepository = childRepository;
    }

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
