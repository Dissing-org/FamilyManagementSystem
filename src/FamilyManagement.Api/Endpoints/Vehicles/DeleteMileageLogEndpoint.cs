using FastEndpoints;
using FamilyManagement.Application.Vehicles;

namespace FamilyManagement.Api.Endpoints.Vehicles;

public class DeleteMileageLogRequest
{
    public Guid Id { get; set; }
    public Guid LogId { get; set; }
}

/// <summary>
/// Endpoint for deleting a mileage log entry for a vehicle.
/// </summary>
public class DeleteMileageLogEndpoint : Endpoint<DeleteMileageLogRequest>
{
    private readonly DeleteMileageLogCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteMileageLogEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler for deleting mileage logs.</param>
    public DeleteMileageLogEndpoint(DeleteMileageLogCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route and OpenAPI metadata for deleting a mileage log entry.
    /// </summary>
    public override void Configure()
    {
        Delete("/api/vehicles/{id}/mileage/{logId}");
        Summary(s =>
        {
            s.Summary = "Delete a mileage log entry";
            s.Description = "Removes a specific mileage log reading from the vehicle history.";
        });
    }

    /// <summary>
    /// Handles the HTTP DELETE request to delete a mileage log entry for a vehicle.
    /// </summary>
    /// <param name="req">The request parameters containing vehicle ID and mileage log ID.</param>
    /// <param name="ct">The cancellation token to observe.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task HandleAsync(DeleteMileageLogRequest req, CancellationToken ct)
    {
        var deleted = await _handler.HandleAsync(new DeleteMileageLogCommand(req.Id, req.LogId), ct);
        if (!deleted)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
            return;
        }

        await HttpContext.Response.SendResultAsync(TypedResults.NoContent());
    }
}
