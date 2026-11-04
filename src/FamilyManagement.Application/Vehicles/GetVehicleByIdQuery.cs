using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

/// <summary>
/// Query to retrieve a vehicle by its unique identifier.
/// </summary>
public record GetVehicleByIdQuery(Guid Id);

/// <summary>
/// Handler for retrieving a vehicle by its unique identifier.
/// </summary>
public class GetVehicleByIdQueryHandler
{
    private readonly IVehicleRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetVehicleByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The vehicle repository.</param>
    public GetVehicleByIdQueryHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles retrieving a vehicle by its unique identifier.
    /// </summary>
    /// <param name="query">The query containing the vehicle identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="VehicleDto"/> if the vehicle was found; otherwise, <see langword="null"/>.</returns>
    public async Task<VehicleDto?> HandleAsync(GetVehicleByIdQuery query, CancellationToken cancellationToken = default)
    {
        var vehicle = await _repository.GetByIdAsync(VehicleId.From(query.Id), cancellationToken);
        return vehicle is null ? null : VehicleDto.FromDomain(vehicle);
    }
}
