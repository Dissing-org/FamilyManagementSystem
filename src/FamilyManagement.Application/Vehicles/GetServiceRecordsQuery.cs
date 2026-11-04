using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

/// <summary>
/// Query to retrieve all service records for a vehicle.
/// </summary>
public record GetServiceRecordsQuery(Guid VehicleId);

/// <summary>
/// Handler for retrieving service records for a vehicle.
/// </summary>
public class GetServiceRecordsQueryHandler
{
    private readonly IVehicleServiceRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetServiceRecordsQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The vehicle service repository.</param>
    public GetServiceRecordsQueryHandler(IVehicleServiceRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles retrieving all service records for a specific vehicle.
    /// </summary>
    /// <param name="query">The query containing the vehicle identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of <see cref="VehicleServiceRecordDto"/> entries for the vehicle.</returns>
    public async Task<List<VehicleServiceRecordDto>> HandleAsync(GetServiceRecordsQuery query, CancellationToken cancellationToken = default)
    {
        var records = await _repository.GetByVehicleIdAsync(VehicleId.From(query.VehicleId), cancellationToken);
        return records.Select(VehicleServiceRecordDto.FromDomain).ToList();
    }
}
