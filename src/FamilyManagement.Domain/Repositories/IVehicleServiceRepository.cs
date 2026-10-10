using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

public interface IVehicleServiceRepository
{
    Task<List<VehicleServiceRecord>> GetByVehicleIdAsync(VehicleId vehicleId, CancellationToken cancellationToken = default);
    Task<VehicleServiceRecord?> GetByIdAsync(ServiceRecordId id, CancellationToken cancellationToken = default);
    Task AddAsync(VehicleServiceRecord serviceRecord, CancellationToken cancellationToken = default);
    Task DeleteAsync(VehicleServiceRecord serviceRecord, CancellationToken cancellationToken = default);
}
