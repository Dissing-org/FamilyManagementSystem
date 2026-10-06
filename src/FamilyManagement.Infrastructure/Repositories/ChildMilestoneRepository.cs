using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using FamilyManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyManagement.Infrastructure.Repositories;

public class ChildMilestoneRepository : IChildMilestoneRepository
{
    private readonly ReceiptDbContext _dbContext;

    public ChildMilestoneRepository(ReceiptDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(ChildMilestone milestone, CancellationToken cancellationToken = default)
    {
        await _dbContext.ChildMilestones.AddAsync(milestone, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<ChildMilestone> milestones, CancellationToken cancellationToken = default)
    {
        await _dbContext.ChildMilestones.AddRangeAsync(milestones, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ChildMilestone?> GetByIdAsync(MilestoneId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ChildMilestones
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<List<ChildMilestone>> GetByChildIdAsync(
        ChildId childId,
        MilestoneStatus? status = null,
        MilestoneCategory? category = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.ChildMilestones
            .Where(m => m.ChildId == childId);

        if (status.HasValue)
        {
            query = query.Where(m => m.Status == status.Value);
        }

        if (category.HasValue)
        {
            query = query.Where(m => m.Category == category.Value);
        }

        return await query
            .OrderBy(m => m.ExpectedAgeMonths)
            .ThenByDescending(m => m.AchievedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(ChildMilestone milestone, CancellationToken cancellationToken = default)
    {
        _dbContext.ChildMilestones.Update(milestone);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ChildMilestone milestone, CancellationToken cancellationToken = default)
    {
        _dbContext.ChildMilestones.Remove(milestone);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
