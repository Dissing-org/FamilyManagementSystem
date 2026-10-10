using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

public record DeleteVehicleCommand(Guid Id);

public class DeleteVehicleCommandHandler
{
    private readonly IVehicleRepository _repository;

    public DeleteVehicleCommandHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(DeleteVehicleCommand command, CancellationToken cancellationToken = default)
    {
        var vehicle = await _repository.GetByIdAsync(VehicleId.From(command.Id), cancellationToken);
        if (vehicle is null)
        {
            return false;
        }

        await _repository.DeleteAsync(vehicle, cancellationToken);
        return true;
    }
}
