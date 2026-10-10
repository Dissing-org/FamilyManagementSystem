using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using FamilyManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FamilyManagement.Infrastructure.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly ReceiptDbContext _dbContext;

    public VehicleRepository(ReceiptDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Vehicle?> GetByIdAsync(VehicleId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Vehicles
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<List<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Vehicles
            .OrderBy(v => v.Make)
            .ThenBy(v => v.Model)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        await _dbContext.Vehicles.AddAsync(vehicle, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        _dbContext.Vehicles.Update(vehicle);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Vehicle vehicle, CancellationToken cancellationToken = default)
    {
        _dbContext.Vehicles.Remove(vehicle);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
