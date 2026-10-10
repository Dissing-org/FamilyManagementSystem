using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(VehicleId id, CancellationToken cancellationToken = default);
    Task<List<Vehicle>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
    Task UpdateAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
    Task DeleteAsync(Vehicle vehicle, CancellationToken cancellationToken = default);
}
