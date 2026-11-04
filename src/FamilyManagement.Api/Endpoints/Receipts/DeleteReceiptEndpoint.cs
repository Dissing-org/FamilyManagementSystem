using FastEndpoints;
using FamilyManagement.Application.UseCases.Receipts.Commands;

namespace FamilyManagement.Api.Endpoints.Receipts;

/// <summary>
/// Request parameter model for deleting a receipt.
/// </summary>
public class DeleteReceiptRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// Endpoint for archiving/soft-deleting a purchase receipt record.
/// </summary>
public class DeleteReceiptEndpoint : Endpoint<DeleteReceiptRequest>
{
    private readonly DeleteReceiptCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteReceiptEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler used to delete receipts.</param>
    public DeleteReceiptEndpoint(DeleteReceiptCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures HTTP route mapping, OpenAPI documentation metadata, and permissions.
    /// </summary>
    public override void Configure()
    {
        Delete("/api/receipts/{Id:guid}");
        Summary(s =>
        {
            s.Summary = "Delete (archive) a receipt";
            s.Description = "Marks a receipt as archived in SQLite.";
        });
    }

    /// <summary>
    /// Handles the request to delete/archive a receipt asynchronously.
    /// </summary>
    /// <param name="req">The request containing the receipt ID.</param>
    /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
