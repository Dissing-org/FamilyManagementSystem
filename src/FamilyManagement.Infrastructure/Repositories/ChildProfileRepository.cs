using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using FamilyManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyManagement.Infrastructure.Repositories;

public class ChildProfileRepository : IChildProfileRepository
{
    private readonly ReceiptDbContext _dbContext;

    public ChildProfileRepository(ReceiptDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(ChildProfile child, CancellationToken cancellationToken = default)
    {
        await _dbContext.ChildProfiles.AddAsync(child, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ChildProfile?> GetByIdAsync(ChildId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ChildProfiles
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<List<ChildProfile>> ListAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.ChildProfiles
            .OrderBy(c => c.DateOfBirth)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(ChildProfile child, CancellationToken cancellationToken = default)
    {
        _dbContext.ChildProfiles.Update(child);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ChildProfile child, CancellationToken cancellationToken = default)
    {
        _dbContext.ChildProfiles.Remove(child);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
