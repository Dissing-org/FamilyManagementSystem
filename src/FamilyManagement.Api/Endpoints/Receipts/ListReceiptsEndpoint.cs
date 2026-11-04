using FastEndpoints;
using FamilyManagement.Application.DTOs;
using FamilyManagement.Application.UseCases.Receipts.Queries;

namespace FamilyManagement.Api.Endpoints.Receipts;

/// <summary>
/// Request parameter model for listing receipts, with optional merchant filter.
/// </summary>
public class ListReceiptsRequest
{
    public string? Merchant { get; set; }
}

/// <summary>
/// Endpoint for listing active receipts with optional filtering by merchant name.
/// </summary>
public class ListReceiptsEndpoint : Endpoint<ListReceiptsRequest, IReadOnlyList<ReceiptResponseDto>>
{
    private readonly ListReceiptsQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListReceiptsEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler used to retrieve receipts.</param>
    public ListReceiptsEndpoint(ListReceiptsQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures HTTP route mapping, OpenAPI documentation metadata, and permissions.
    /// </summary>
    public override void Configure()
    {
        Get("/api/receipts");
        Summary(s =>
        {
            s.Summary = "List all active receipts";
            s.Description = "Retrieves active receipts with optional filtering by merchant.";
        });
    }

    /// <summary>
    /// Handles the request to list receipts asynchronously.
    /// </summary>
    /// <param name="req">The request containing optional filtering criteria.</param>
    /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(ListReceiptsRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new ListReceiptsQuery(req.Merchant), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
