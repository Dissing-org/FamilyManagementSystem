using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

/// <summary>
/// Query to retrieve all mileage logs for a vehicle.
/// </summary>
public record GetMileageLogsQuery(Guid VehicleId);

/// <summary>
/// Handler for retrieving mileage logs for a vehicle.
/// </summary>
public class GetMileageLogsQueryHandler
{
    private readonly IMileageLogRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMileageLogsQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The mileage log repository.</param>
    public GetMileageLogsQueryHandler(IMileageLogRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles retrieving all mileage log entries for a specific vehicle.
    /// </summary>
    /// <param name="query">The query containing the vehicle identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A list of <see cref="MileageLogDto"/> entries for the vehicle.</returns>
    public async Task<List<MileageLogDto>> HandleAsync(GetMileageLogsQuery query, CancellationToken cancellationToken = default)
    {
        var logs = await _repository.GetByVehicleIdAsync(VehicleId.From(query.VehicleId), cancellationToken);
        return logs.Select(MileageLogDto.FromDomain).ToList();
    }
}
