using FastEndpoints;
using FamilyManagement.Application.Vehicles;

namespace FamilyManagement.Api.Endpoints.Vehicles;

public class GetVehicleByIdRequest
{
    public Guid Id { get; set; }
}

public class GetVehicleByIdEndpoint : Endpoint<GetVehicleByIdRequest, VehicleDto>
{
    private readonly GetVehicleByIdQueryHandler _handler;

    public GetVehicleByIdEndpoint(GetVehicleByIdQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/vehicles/{id}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get vehicle details by ID";
            s.Description = "Retrieves a single vehicle's details and configuration.";
        });
    }

    public override async Task HandleAsync(GetVehicleByIdRequest req, CancellationToken ct)
    {
        var vehicle = await _handler.HandleAsync(new GetVehicleByIdQuery(req.Id), ct);
        if (vehicle is null)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
            return;
        }

        await HttpContext.Response.SendResultAsync(TypedResults.Ok(vehicle));
    }
}
