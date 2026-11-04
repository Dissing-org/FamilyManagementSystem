using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

/// <summary>
/// Command to delete a vehicle by its unique identifier.
/// </summary>
public record DeleteVehicleCommand(Guid Id);

/// <summary>
/// Handler for deleting a vehicle.
/// </summary>
public class DeleteVehicleCommandHandler
{
    private readonly IVehicleRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteVehicleCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The vehicle repository.</param>
    public DeleteVehicleCommandHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles the deletion of a vehicle.
    /// </summary>
    /// <param name="command">The command containing the vehicle identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><see langword="true"/> if the vehicle was found and deleted; otherwise, <see langword="false"/>.</returns>
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
