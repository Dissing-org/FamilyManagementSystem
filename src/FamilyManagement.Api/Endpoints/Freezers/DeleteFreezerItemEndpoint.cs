using FastEndpoints;
using FamilyManagement.Application.UseCases.Freezers.Commands;

namespace FamilyManagement.Api.Endpoints.Freezers;

public class DeleteFreezerItemRequest
{
    public Guid Id { get; set; }
}

public class DeleteFreezerItemEndpoint : Endpoint<DeleteFreezerItemRequest>
{
    private readonly DeleteFreezerItemCommandHandler _handler;

    public DeleteFreezerItemEndpoint(DeleteFreezerItemCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Delete("/api/freezer-items/{id}");
        Summary(s =>
        {
            s.Summary = "Delete a freezer item";
            s.Description = "Permanently removes an item and its transaction history.";
        });
    }

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
