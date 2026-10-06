using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Commands;

public record MarkMilestoneAchievedCommand(
    Guid Id,
    DateTime AchievedDate,
    string? Notes = null,
    string? PhotoUrl = null);

public class MarkMilestoneAchievedCommandHandler
{
    private readonly IChildMilestoneRepository _repository;
    private readonly IChildProfileRepository _childRepository;

    public MarkMilestoneAchievedCommandHandler(
        IChildMilestoneRepository repository,
        IChildProfileRepository childRepository)
    {
        _repository = repository;
        _childRepository = childRepository;
    }

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
