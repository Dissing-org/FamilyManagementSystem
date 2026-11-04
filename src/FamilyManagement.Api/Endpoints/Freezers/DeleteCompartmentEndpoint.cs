using FastEndpoints;
using FamilyManagement.Application.UseCases.Freezers.Commands;

namespace FamilyManagement.Api.Endpoints.Freezers;

/// <summary>
/// Request parameters identifying the freezer and compartment to delete.
/// </summary>
public class DeleteCompartmentRequest
{
    public Guid FreezerId { get; set; }
    public Guid CompartmentId { get; set; }
}

/// <summary>
/// Endpoint that removes a compartment or drawer from a freezer.
/// </summary>
public class DeleteCompartmentEndpoint : Endpoint<DeleteCompartmentRequest>
{
    private readonly DeleteCompartmentCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteCompartmentEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler used to process compartment deletion.</param>
    public DeleteCompartmentEndpoint(DeleteCompartmentCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route mapping and OpenAPI specification for the endpoint.
    /// </summary>
    public override void Configure()
    {
        Delete("/api/freezers/{freezerId}/compartments/{compartmentId}");
        Summary(s =>
        {
            s.Summary = "Delete a freezer compartment";
            s.Description = "Removes a compartment/drawer from a freezer.";
        });
    }

    /// <summary>
    /// Handles the request to remove a freezer compartment.
    /// </summary>
    /// <param name="req">The delete request containing freezer and compartment identifiers.</param>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(DeleteCompartmentRequest req, CancellationToken ct)
    {
        var deleted = await _handler.HandleAsync(new DeleteCompartmentCommand(req.FreezerId, req.CompartmentId), ct);
        if (!deleted)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound(new { error = $"Freezer or compartment not found." }));
            return;
        }

        await HttpContext.Response.SendResultAsync(TypedResults.NoContent());
    }
}
