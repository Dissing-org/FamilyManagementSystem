using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

/// <summary>
/// Command to delete a service record for a vehicle.
/// </summary>
public record DeleteVehicleServiceRecordCommand(Guid VehicleId, Guid ServiceId);

/// <summary>
/// Handler for deleting a vehicle service record.
/// </summary>
public class DeleteVehicleServiceRecordCommandHandler
{
    private readonly IVehicleServiceRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteVehicleServiceRecordCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The vehicle service repository.</param>
    public DeleteVehicleServiceRecordCommandHandler(IVehicleServiceRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles the deletion of a vehicle service record.
    /// </summary>
    /// <param name="command">The command containing the vehicle and service record identifiers.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><see langword="true"/> if the service record was found and deleted; otherwise, <see langword="false"/>.</returns>
    public async Task<bool> HandleAsync(DeleteVehicleServiceRecordCommand command, CancellationToken cancellationToken = default)
    {
        var vehicleId = VehicleId.From(command.VehicleId);
        var record = await _repository.GetByIdAsync(ServiceRecordId.From(command.ServiceId), cancellationToken);
        if (record is null || record.VehicleId != vehicleId)
        {
            return false;
        }

        await _repository.DeleteAsync(record, cancellationToken);
        return true;
    }
}
