using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using FamilyManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyManagement.Infrastructure.Repositories;

public class GrowthMeasurementRepository : IGrowthMeasurementRepository
{
    private readonly ReceiptDbContext _dbContext;

    public GrowthMeasurementRepository(ReceiptDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(GrowthMeasurement measurement, CancellationToken cancellationToken = default)
    {
        await _dbContext.GrowthMeasurements.AddAsync(measurement, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<GrowthMeasurement?> GetByIdAsync(GrowthMeasurementId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.GrowthMeasurements
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<List<GrowthMeasurement>> GetByChildIdAsync(ChildId childId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.GrowthMeasurements
            .Where(g => g.ChildId == childId)
            .OrderByDescending(g => g.RecordedDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<GrowthMeasurement?> GetLatestByChildIdAsync(ChildId childId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.GrowthMeasurements
            .Where(g => g.ChildId == childId)
            .OrderByDescending(g => g.RecordedDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task UpdateAsync(GrowthMeasurement measurement, CancellationToken cancellationToken = default)
    {
        _dbContext.GrowthMeasurements.Update(measurement);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(GrowthMeasurement measurement, CancellationToken cancellationToken = default)
    {
        _dbContext.GrowthMeasurements.Remove(measurement);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
