using FastEndpoints;
using FamilyManagement.Application.Vehicles;

namespace FamilyManagement.Api.Endpoints.Vehicles;

public class DeleteVehicleRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// Endpoint for deleting a vehicle.
/// </summary>
public class DeleteVehicleEndpoint : Endpoint<DeleteVehicleRequest>
{
    private readonly DeleteVehicleCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteVehicleEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler for deleting vehicles.</param>
    public DeleteVehicleEndpoint(DeleteVehicleCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route and OpenAPI metadata for deleting a vehicle.
    /// </summary>
    public override void Configure()
    {
        Delete("/api/vehicles/{id}");
        Summary(s =>
        {
            s.Summary = "Delete a vehicle";
            s.Description = "Removes a vehicle from the system.";
        });
    }

    /// <summary>
    /// Handles the HTTP DELETE request to delete a vehicle by its unique identifier.
    /// </summary>
    /// <param name="req">The request containing the identifier of the vehicle to delete.</param>
    /// <param name="ct">The cancellation token to observe.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task HandleAsync(DeleteVehicleRequest req, CancellationToken ct)
    {
        var deleted = await _handler.HandleAsync(new DeleteVehicleCommand(req.Id), ct);
        if (!deleted)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
            return;
        }

        await HttpContext.Response.SendResultAsync(TypedResults.NoContent());
    }
}
