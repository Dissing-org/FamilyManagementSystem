using FastEndpoints;
using FamilyManagement.Application.Vehicles;

namespace FamilyManagement.Api.Endpoints.Vehicles;

public class GetServiceRecordsRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// Endpoint for retrieving service history records for a vehicle.
/// </summary>
public class GetServiceRecordsEndpoint : Endpoint<GetServiceRecordsRequest, List<VehicleServiceRecordDto>>
{
    private readonly GetServiceRecordsQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetServiceRecordsEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler for retrieving vehicle service records.</param>
    public GetServiceRecordsEndpoint(GetServiceRecordsQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route and OpenAPI metadata for retrieving service records for a vehicle.
    /// </summary>
    public override void Configure()
    {
        Get("/api/vehicles/{id}/services");
        Summary(s =>
        {
            s.Summary = "Get service history for a vehicle";
            s.Description = "Retrieves all recorded maintenance and repairs for a vehicle, ordered by date descending.";
        });
    }

    /// <summary>
    /// Handles the HTTP GET request to retrieve service history records for a vehicle.
    /// </summary>
    /// <param name="req">The request containing the vehicle identifier.</param>
    /// <param name="ct">The cancellation token to observe.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task HandleAsync(GetServiceRecordsRequest req, CancellationToken ct)
    {
        var records = await _handler.HandleAsync(new GetServiceRecordsQuery(req.Id), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(records));
    }
}
