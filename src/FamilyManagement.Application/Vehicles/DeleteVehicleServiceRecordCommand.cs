using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

public record DeleteVehicleServiceRecordCommand(Guid VehicleId, Guid ServiceId);

public class DeleteVehicleServiceRecordCommandHandler
{
    private readonly IVehicleServiceRepository _repository;

    public DeleteVehicleServiceRecordCommandHandler(IVehicleServiceRepository repository)
    {
        _repository = repository;
    }

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
