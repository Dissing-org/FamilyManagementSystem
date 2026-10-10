using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using FamilyManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyManagement.Infrastructure.Repositories;

public class MileageLogRepository : IMileageLogRepository
{
    private readonly ReceiptDbContext _dbContext;

    public MileageLogRepository(ReceiptDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<MileageLogEntry>> GetByVehicleIdAsync(VehicleId vehicleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MileageLogs
            .Where(m => m.VehicleId == vehicleId)
            .OrderByDescending(m => m.RecordedDate)
            .ThenByDescending(m => m.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<MileageLogEntry?> GetByIdAsync(MileageLogId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.MileageLogs
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task AddAsync(MileageLogEntry logEntry, CancellationToken cancellationToken = default)
    {
        await _dbContext.MileageLogs.AddAsync(logEntry, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(MileageLogEntry logEntry, CancellationToken cancellationToken = default)
    {
        _dbContext.MileageLogs.Remove(logEntry);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
