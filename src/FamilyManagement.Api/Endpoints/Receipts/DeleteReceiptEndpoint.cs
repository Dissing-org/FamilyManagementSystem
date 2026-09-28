using FastEndpoints;
using FamilyManagement.Application.UseCases.Receipts.Commands;

namespace FamilyManagement.Api.Endpoints.Receipts;

public class DeleteReceiptRequest
{
    public Guid Id { get; set; }
}

public class DeleteReceiptEndpoint : Endpoint<DeleteReceiptRequest>
{
    private readonly DeleteReceiptCommandHandler _handler;

    public DeleteReceiptEndpoint(DeleteReceiptCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Delete("/api/receipts/{Id:guid}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Delete (archive) a receipt";
            s.Description = "Marks a receipt as archived in SQLite.";
        });
    }

    public override async Task HandleAsync(DeleteReceiptRequest req, CancellationToken ct)
    {
        var deleted = await _handler.HandleAsync(new DeleteReceiptCommand(req.Id), ct);
        if (!deleted)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
            return;
        }

        await HttpContext.Response.SendResultAsync(TypedResults.NoContent());
    }
}
