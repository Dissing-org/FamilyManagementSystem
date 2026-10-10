using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

public record GetVehicleSummaryQuery(Guid VehicleId);

public class GetVehicleSummaryQueryHandler
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMileageLogRepository _mileageRepository;
    private readonly IVehicleServiceRepository _serviceRepository;

    public GetVehicleSummaryQueryHandler(
        IVehicleRepository vehicleRepository,
        IMileageLogRepository mileageRepository,
        IVehicleServiceRepository serviceRepository)
    {
        _vehicleRepository = vehicleRepository;
        _mileageRepository = mileageRepository;
        _serviceRepository = serviceRepository;
    }

    public async Task<VehicleSummaryDto> HandleAsync(GetVehicleSummaryQuery query, CancellationToken cancellationToken = default)
    {
        var vehicleId = VehicleId.From(query.VehicleId);
        var vehicle = await _vehicleRepository.GetByIdAsync(vehicleId, cancellationToken);
        if (vehicle is null)
        {
            throw new KeyNotFoundException($"Vehicle with ID '{query.VehicleId}' was not found.");
        }

        var mileageLogs = await _mileageRepository.GetByVehicleIdAsync(vehicleId, cancellationToken);
        var serviceRecords = await _serviceRepository.GetByVehicleIdAsync(vehicleId, cancellationToken);

        return VehicleSummaryDto.Create(vehicle, mileageLogs, serviceRecords);
    }
}
