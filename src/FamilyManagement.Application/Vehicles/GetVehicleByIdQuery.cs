using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

public record GetVehicleByIdQuery(Guid Id);

public class GetVehicleByIdQueryHandler
{
    private readonly IVehicleRepository _repository;

    public GetVehicleByIdQueryHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public async Task<VehicleDto?> HandleAsync(GetVehicleByIdQuery query, CancellationToken cancellationToken = default)
    {
        var vehicle = await _repository.GetByIdAsync(VehicleId.From(query.Id), cancellationToken);
        return vehicle is null ? null : VehicleDto.FromDomain(vehicle);
    }
}
