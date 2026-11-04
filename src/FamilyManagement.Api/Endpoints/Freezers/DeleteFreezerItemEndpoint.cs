using FastEndpoints;
using FamilyManagement.Application.UseCases.Freezers.Commands;

namespace FamilyManagement.Api.Endpoints.Freezers;

/// <summary>
/// Request parameters identifying the freezer item to delete.
/// </summary>
public class DeleteFreezerItemRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// Endpoint that permanently deletes a freezer item and its consumption history.
/// </summary>
public class DeleteFreezerItemEndpoint : Endpoint<DeleteFreezerItemRequest>
{
    private readonly DeleteFreezerItemCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteFreezerItemEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler used to process freezer item deletion.</param>
    public DeleteFreezerItemEndpoint(DeleteFreezerItemCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route mapping and OpenAPI specification for the endpoint.
    /// </summary>
    public override void Configure()
    {
        Delete("/api/freezer-items/{id}");
        Summary(s =>
        {
            s.Summary = "Delete a freezer item";
            s.Description = "Permanently removes an item and its transaction history.";
        });
    }

    /// <summary>
    /// Handles the request to permanently delete a freezer item.
    /// </summary>
    /// <param name="req">The delete request containing the item ID.</param>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(DeleteFreezerItemRequest req, CancellationToken ct)
    {
        var deleted = await _handler.HandleAsync(new DeleteFreezerItemCommand(req.Id), ct);
        if (!deleted)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound(new { error = $"Item '{req.Id}' not found." }));
            return;
        }

        await HttpContext.Response.SendResultAsync(TypedResults.NoContent());
    }
}
