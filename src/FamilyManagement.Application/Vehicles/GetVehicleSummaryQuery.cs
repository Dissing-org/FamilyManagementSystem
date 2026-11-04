using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

/// <summary>
/// Query to retrieve a comprehensive summary for a vehicle.
/// </summary>
public record GetVehicleSummaryQuery(Guid VehicleId);

/// <summary>
/// Handler for retrieving a comprehensive vehicle summary.
/// </summary>
public class GetVehicleSummaryQueryHandler
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMileageLogRepository _mileageRepository;
    private readonly IVehicleServiceRepository _serviceRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetVehicleSummaryQueryHandler"/> class.
    /// </summary>
    /// <param name="vehicleRepository">The vehicle repository.</param>
    /// <param name="mileageRepository">The mileage log repository.</param>
    /// <param name="serviceRepository">The vehicle service repository.</param>
    public GetVehicleSummaryQueryHandler(
        IVehicleRepository vehicleRepository,
        IMileageLogRepository mileageRepository,
        IVehicleServiceRepository serviceRepository)
    {
        _vehicleRepository = vehicleRepository;
        _mileageRepository = mileageRepository;
        _serviceRepository = serviceRepository;
    }

    /// <summary>
    /// Handles retrieving a comprehensive summary of a vehicle including maintenance status, upcoming services, and alerts.
    /// </summary>
    /// <param name="query">The query containing the vehicle identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="VehicleSummaryDto"/> containing the vehicle summary and status alerts.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the vehicle with the specified identifier is not found.</exception>
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
