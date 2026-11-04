using FastEndpoints;
using FamilyManagement.Application.Vehicles;

namespace FamilyManagement.Api.Endpoints.Vehicles;

public class GetMileageLogsRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// Endpoint for retrieving mileage logs for a vehicle.
/// </summary>
public class GetMileageLogsEndpoint : Endpoint<GetMileageLogsRequest, List<MileageLogDto>>
{
    private readonly GetMileageLogsQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMileageLogsEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler for retrieving mileage logs.</param>
    public GetMileageLogsEndpoint(GetMileageLogsQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route and OpenAPI metadata for retrieving mileage logs for a vehicle.
    /// </summary>
    public override void Configure()
    {
        Get("/api/vehicles/{id}/mileage");
        Summary(s =>
        {
            s.Summary = "Get mileage logs for a vehicle";
            s.Description = "Retrieves all odometer readings for a specific vehicle, ordered by date descending.";
        });
    }

    /// <summary>
    /// Handles the HTTP GET request to retrieve all mileage log readings for a vehicle.
    /// </summary>
    /// <param name="req">The request containing the vehicle identifier.</param>
    /// <param name="ct">The cancellation token to observe.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task HandleAsync(GetMileageLogsRequest req, CancellationToken ct)
    {
        var logs = await _handler.HandleAsync(new GetMileageLogsQuery(req.Id), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(logs));
    }
}
