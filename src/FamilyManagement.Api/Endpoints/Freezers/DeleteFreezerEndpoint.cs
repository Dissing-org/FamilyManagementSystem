using FastEndpoints;
using FamilyManagement.Application.UseCases.Freezers.Commands;

namespace FamilyManagement.Api.Endpoints.Freezers;

public class DeleteFreezerRequest
{
    public Guid Id { get; set; }
}

public class DeleteFreezerEndpoint : Endpoint<DeleteFreezerRequest>
{
    private readonly DeleteFreezerCommandHandler _handler;

    public DeleteFreezerEndpoint(DeleteFreezerCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Delete("/api/freezers/{id}");
        Summary(s =>
        {
            s.Summary = "Delete a freezer";
            s.Description = "Removes a freezer and its compartments/items from the system.";
        });
    }

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
