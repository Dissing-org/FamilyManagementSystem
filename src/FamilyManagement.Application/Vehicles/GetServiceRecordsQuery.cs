using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

public record GetServiceRecordsQuery(Guid VehicleId);

public class GetServiceRecordsQueryHandler
{
    private readonly IVehicleServiceRepository _repository;

    public GetServiceRecordsQueryHandler(IVehicleServiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<VehicleServiceRecordDto>> HandleAsync(GetServiceRecordsQuery query, CancellationToken cancellationToken = default)
    {
        var records = await _repository.GetByVehicleIdAsync(VehicleId.From(query.VehicleId), cancellationToken);
        return records.Select(VehicleServiceRecordDto.FromDomain).ToList();
    }
}
