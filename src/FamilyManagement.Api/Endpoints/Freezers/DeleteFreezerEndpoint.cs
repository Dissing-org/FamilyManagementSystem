using FastEndpoints;
using FamilyManagement.Application.UseCases.Freezers.Commands;

namespace FamilyManagement.Api.Endpoints.Freezers;

/// <summary>
/// Request parameters identifying the freezer to delete.
/// </summary>
public class DeleteFreezerRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// Endpoint that deletes an entire freezer and its compartments and items.
/// </summary>
public class DeleteFreezerEndpoint : Endpoint<DeleteFreezerRequest>
{
    private readonly DeleteFreezerCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteFreezerEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler used to process freezer deletion.</param>
    public DeleteFreezerEndpoint(DeleteFreezerCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route mapping and OpenAPI specification for the endpoint.
    /// </summary>
    public override void Configure()
    {
        Delete("/api/freezers/{id}");
        Summary(s =>
        {
            s.Summary = "Delete a freezer";
            s.Description = "Removes a freezer and its compartments/items from the system.";
        });
    }

    /// <summary>
    /// Handles the request to delete a freezer unit.
    /// </summary>
    /// <param name="req">The delete request containing the freezer ID.</param>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(DeleteFreezerRequest req, CancellationToken ct)
    {
        var deleted = await _handler.HandleAsync(new DeleteFreezerCommand(req.Id), ct);
        if (!deleted)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound(new { error = $"Freezer '{req.Id}' not found." }));
            return;
        }

        await HttpContext.Response.SendResultAsync(TypedResults.NoContent());
    }
}
