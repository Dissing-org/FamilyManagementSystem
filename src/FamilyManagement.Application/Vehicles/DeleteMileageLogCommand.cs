using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

public record DeleteMileageLogCommand(Guid VehicleId, Guid LogId);

public class DeleteMileageLogCommandHandler
{
    private readonly IMileageLogRepository _mileageLogRepository;
    private readonly IVehicleRepository _vehicleRepository;

    public DeleteMileageLogCommandHandler(
        IMileageLogRepository mileageLogRepository,
        IVehicleRepository vehicleRepository)
    {
        _mileageLogRepository = mileageLogRepository;
        _vehicleRepository = vehicleRepository;
    }

    public async Task<bool> HandleAsync(DeleteMileageLogCommand command, CancellationToken cancellationToken = default)
    {
        var vehicleId = VehicleId.From(command.VehicleId);
        var log = await _mileageLogRepository.GetByIdAsync(MileageLogId.From(command.LogId), cancellationToken);
        if (log is null || log.VehicleId != vehicleId)
        {
            return false;
        }

        await _mileageLogRepository.DeleteAsync(log, cancellationToken);

        // Recalculate and synchronize vehicle's current mileage from remaining history
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId, cancellationToken);
        if (vehicle != null)
        {
            var remainingLogs = await _mileageLogRepository.GetByVehicleIdAsync(vehicleId, cancellationToken);
            var maxRemainingMileage = remainingLogs.Count > 0 ? remainingLogs.Max(l => l.MileageKm) : 0;

            if (maxRemainingMileage < vehicle.CurrentMileageKm)
            {
                vehicle.UpdateCurrentMileage(maxRemainingMileage, allowDecrease: true);
                await _vehicleRepository.UpdateAsync(vehicle, cancellationToken);
            }
        }

        return true;
    }
}
