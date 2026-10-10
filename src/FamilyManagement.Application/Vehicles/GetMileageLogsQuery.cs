using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

public record GetMileageLogsQuery(Guid VehicleId);

public class GetMileageLogsQueryHandler
{
    private readonly IMileageLogRepository _repository;

    public GetMileageLogsQueryHandler(IMileageLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<MileageLogDto>> HandleAsync(GetMileageLogsQuery query, CancellationToken cancellationToken = default)
    {
        var logs = await _repository.GetByVehicleIdAsync(VehicleId.From(query.VehicleId), cancellationToken);
        return logs.Select(MileageLogDto.FromDomain).ToList();
    }
}
