using FastEndpoints;
using FamilyManagement.Application.Vehicles;

namespace FamilyManagement.Api.Endpoints.Vehicles;

public class DeleteVehicleServiceRecordRequest
{
    public Guid Id { get; set; }
    public Guid ServiceId { get; set; }
}

/// <summary>
/// Endpoint for deleting a service record for a vehicle.
/// </summary>
public class DeleteVehicleServiceRecordEndpoint : Endpoint<DeleteVehicleServiceRecordRequest>
{
    private readonly DeleteVehicleServiceRecordCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteVehicleServiceRecordEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler for deleting vehicle service records.</param>
    public DeleteVehicleServiceRecordEndpoint(DeleteVehicleServiceRecordCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route and OpenAPI metadata for deleting a vehicle service record.
    /// </summary>
    public override void Configure()
    {
        Delete("/api/vehicles/{id}/services/{serviceId}");
        Summary(s =>
        {
            s.Summary = "Delete a service record";
            s.Description = "Removes a specific service or repair entry from the vehicle maintenance log.";
        });
    }

    /// <summary>
    /// Handles the HTTP DELETE request to delete a maintenance or repair service record for a vehicle.
    /// </summary>
    /// <param name="req">The request parameters containing vehicle ID and service record ID.</param>
    /// <param name="ct">The cancellation token to observe.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task HandleAsync(DeleteVehicleServiceRecordRequest req, CancellationToken ct)
    {
        var deleted = await _handler.HandleAsync(new DeleteVehicleServiceRecordCommand(req.Id, req.ServiceId), ct);
        if (!deleted)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
            return;
        }

        await HttpContext.Response.SendResultAsync(TypedResults.NoContent());
    }
}
