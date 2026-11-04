using FastEndpoints;
using FamilyManagement.Application.UseCases.Children.Commands;

namespace FamilyManagement.Api.Endpoints.Children;

/// <summary>
/// Request parameter model for deleting a growth measurement.
/// </summary>
public class DeleteGrowthMeasurementRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// Endpoint for deleting a recorded growth measurement.
/// </summary>
public class DeleteGrowthMeasurementEndpoint : Endpoint<DeleteGrowthMeasurementRequest>
{
    private readonly DeleteGrowthMeasurementCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteGrowthMeasurementEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler used to delete growth measurements.</param>
    public DeleteGrowthMeasurementEndpoint(DeleteGrowthMeasurementCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures HTTP route mapping, OpenAPI documentation metadata, and permissions.
    /// </summary>
    public override void Configure()
    {
        Delete("/api/children/growth/{id}");
        Summary(s =>
        {
            s.Summary = "Delete a growth measurement";
            s.Description = "Deletes a previously recorded height/weight measurement.";
        });
    }

    /// <summary>
    /// Handles the deletion of a growth measurement asynchronously.
    /// </summary>
    /// <param name="req">The request containing the growth measurement ID.</param>
    /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(DeleteGrowthMeasurementRequest req, CancellationToken ct)
    {
        var deleted = await _handler.HandleAsync(new DeleteGrowthMeasurementCommand(req.Id), ct);
        if (deleted)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NoContent());
        }
        else
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
        }
    }
}
