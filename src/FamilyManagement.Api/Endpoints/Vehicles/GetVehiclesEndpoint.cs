using FastEndpoints;
using FamilyManagement.Application.Vehicles;

namespace FamilyManagement.Api.Endpoints.Vehicles;

public class GetVehiclesEndpoint : EndpointWithoutRequest<List<VehicleDto>>
{
    private readonly GetAllVehiclesQueryHandler _handler;

    public GetVehiclesEndpoint(GetAllVehiclesQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/vehicles");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get all registered vehicles";
            s.Description = "Retrieves all family vehicles with their current mileage and details.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var vehicles = await _handler.HandleAsync(new GetAllVehiclesQuery(), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(vehicles));
    }
}
