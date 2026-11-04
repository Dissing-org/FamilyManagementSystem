using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.Vehicles;

/// <summary>
/// Query to retrieve all registered vehicles.
/// </summary>
public record GetAllVehiclesQuery();

/// <summary>
/// Handler for retrieving all registered vehicles.
/// </summary>
public class GetAllVehiclesQueryHandler
{
    private readonly IVehicleRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllVehiclesQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The vehicle repository.</param>
    public GetAllVehiclesQueryHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles retrieving all registered vehicles.
    /// </summary>
    /// <param name="query">The query to retrieve all vehicles.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of <see cref="VehicleDto"/> representing all vehicles.</returns>
    public async Task<List<VehicleDto>> HandleAsync(GetAllVehiclesQuery query, CancellationToken cancellationToken = default)
    {
        var vehicles = await _repository.GetAllAsync(cancellationToken);
        return vehicles.Select(VehicleDto.FromDomain).ToList();
    }
}
