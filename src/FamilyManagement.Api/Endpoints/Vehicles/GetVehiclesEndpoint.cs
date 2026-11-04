using FastEndpoints;
using FamilyManagement.Application.Vehicles;

namespace FamilyManagement.Api.Endpoints.Vehicles;

/// <summary>
/// Endpoint for retrieving all registered vehicles.
/// </summary>
public class GetVehiclesEndpoint : EndpointWithoutRequest<List<VehicleDto>>
{
    private readonly GetAllVehiclesQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetVehiclesEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler for retrieving all vehicles.</param>
    public GetVehiclesEndpoint(GetAllVehiclesQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route and OpenAPI metadata for retrieving all registered vehicles.
    /// </summary>
    public override void Configure()
    {
        Get("/api/vehicles");
        Summary(s =>
        {
            s.Summary = "Get all registered vehicles";
            s.Description = "Retrieves all family vehicles with their current mileage and details.";
        });
    }

    /// <summary>
    /// Handles the HTTP GET request to retrieve all registered vehicles.
    /// </summary>
    /// <param name="ct">The cancellation token to observe.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task HandleAsync(CancellationToken ct)
    {
        var vehicles = await _handler.HandleAsync(new GetAllVehiclesQuery(), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(vehicles));
    }
}
