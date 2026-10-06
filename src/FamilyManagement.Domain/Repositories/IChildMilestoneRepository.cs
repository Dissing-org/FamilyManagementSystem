using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

public interface IChildMilestoneRepository
{
    Task AddAsync(ChildMilestone milestone, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<ChildMilestone> milestones, CancellationToken cancellationToken = default);
    Task<ChildMilestone?> GetByIdAsync(MilestoneId id, CancellationToken cancellationToken = default);
    Task<List<ChildMilestone>> GetByChildIdAsync(
        ChildId childId,
        MilestoneStatus? status = null,
        MilestoneCategory? category = null,
        CancellationToken cancellationToken = default);
    Task UpdateAsync(ChildMilestone milestone, CancellationToken cancellationToken = default);
    Task DeleteAsync(ChildMilestone milestone, CancellationToken cancellationToken = default);
}
