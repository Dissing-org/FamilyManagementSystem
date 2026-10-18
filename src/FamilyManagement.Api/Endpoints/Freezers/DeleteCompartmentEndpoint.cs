using FastEndpoints;
using FamilyManagement.Application.UseCases.Freezers.Commands;

namespace FamilyManagement.Api.Endpoints.Freezers;

public class DeleteCompartmentRequest
{
    public Guid FreezerId { get; set; }
    public Guid CompartmentId { get; set; }
}

public class DeleteCompartmentEndpoint : Endpoint<DeleteCompartmentRequest>
{
    private readonly DeleteCompartmentCommandHandler _handler;

    public DeleteCompartmentEndpoint(DeleteCompartmentCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Delete("/api/freezers/{freezerId}/compartments/{compartmentId}");
        Summary(s =>
        {
            s.Summary = "Delete a freezer compartment";
            s.Description = "Removes a compartment/drawer from a freezer.";
        });
    }

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
