using FastEndpoints;
using FamilyManagement.Application.Vehicles;

namespace FamilyManagement.Api.Endpoints.Vehicles;

public class GetVehicleByIdRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// Endpoint for retrieving a vehicle by its unique identifier.
/// </summary>
public class GetVehicleByIdEndpoint : Endpoint<GetVehicleByIdRequest, VehicleDto>
{
    private readonly GetVehicleByIdQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetVehicleByIdEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler for retrieving vehicle details by ID.</param>
    public GetVehicleByIdEndpoint(GetVehicleByIdQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route and OpenAPI metadata for retrieving vehicle details by ID.
    /// </summary>
    public override void Configure()
    {
        Get("/api/vehicles/{id}");
        Summary(s =>
        {
            s.Summary = "Get vehicle details by ID";
            s.Description = "Retrieves a single vehicle's details and configuration.";
        });
    }

    /// <summary>
    /// Handles the HTTP GET request to retrieve details of a specific vehicle.
    /// </summary>
    /// <param name="req">The request containing the vehicle identifier.</param>
    /// <param name="ct">The cancellation token to observe.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
