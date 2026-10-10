using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using FamilyManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyManagement.Infrastructure.Repositories;

public class VehicleServiceRepository : IVehicleServiceRepository
{
    private readonly ReceiptDbContext _dbContext;

    public VehicleServiceRepository(ReceiptDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<VehicleServiceRecord>> GetByVehicleIdAsync(VehicleId vehicleId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.VehicleServices
            .Where(s => s.VehicleId == vehicleId)
            .OrderByDescending(s => s.ServiceDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<VehicleServiceRecord?> GetByIdAsync(ServiceRecordId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.VehicleServices
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task AddAsync(VehicleServiceRecord serviceRecord, CancellationToken cancellationToken = default)
    {
        await _dbContext.VehicleServices.AddAsync(serviceRecord, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(VehicleServiceRecord serviceRecord, CancellationToken cancellationToken = default)
    {
        _dbContext.VehicleServices.Remove(serviceRecord);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
