using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.Vehicles;

public record GetAllVehiclesQuery();

public class GetAllVehiclesQueryHandler
{
    private readonly IVehicleRepository _repository;

    public GetAllVehiclesQueryHandler(IVehicleRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<VehicleDto>> HandleAsync(GetAllVehiclesQuery query, CancellationToken cancellationToken = default)
    {
        var vehicles = await _repository.GetAllAsync(cancellationToken);
        return vehicles.Select(VehicleDto.FromDomain).ToList();
    }
}
