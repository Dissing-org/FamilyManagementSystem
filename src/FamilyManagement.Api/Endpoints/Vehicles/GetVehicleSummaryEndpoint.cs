using FastEndpoints;
using FamilyManagement.Application.Vehicles;

namespace FamilyManagement.Api.Endpoints.Vehicles;

public class GetVehicleSummaryRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// Endpoint for retrieving the summary dashboard and maintenance status for a vehicle.
/// </summary>
public class GetVehicleSummaryEndpoint : Endpoint<GetVehicleSummaryRequest, VehicleSummaryDto>
{
    private readonly GetVehicleSummaryQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetVehicleSummaryEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler for retrieving vehicle summary metrics.</param>
    public GetVehicleSummaryEndpoint(GetVehicleSummaryQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route and OpenAPI metadata for retrieving the vehicle summary dashboard.
    /// </summary>
    public override void Configure()
    {
        Get("/api/vehicles/{id}/summary");
        Summary(s =>
        {
            s.Summary = "Get vehicle summary dashboard";
            s.Description = "Retrieves an aggregated summary including upcoming service interval countdowns (days and km), inspection due dates, maintenance alerts, and total maintenance costs.";
        });
    }

    /// <summary>
    /// Handles the HTTP GET request to retrieve summary dashboard information for a vehicle.
    /// </summary>
    /// <param name="req">The request containing the vehicle identifier.</param>
    /// <param name="ct">The cancellation token to observe.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task HandleAsync(GetVehicleSummaryRequest req, CancellationToken ct)
    {
        try
        {
            var summary = await _handler.HandleAsync(new GetVehicleSummaryQuery(req.Id), ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Ok(summary));
        }
        catch (KeyNotFoundException)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
        }
    }
}
