using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

public interface IMileageLogRepository
{
    Task<List<MileageLogEntry>> GetByVehicleIdAsync(VehicleId vehicleId, CancellationToken cancellationToken = default);
    Task<MileageLogEntry?> GetByIdAsync(MileageLogId id, CancellationToken cancellationToken = default);
    Task AddAsync(MileageLogEntry logEntry, CancellationToken cancellationToken = default);
    Task DeleteAsync(MileageLogEntry logEntry, CancellationToken cancellationToken = default);
}
