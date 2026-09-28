using FastEndpoints;
using FamilyManagement.Application.DTOs;
using FamilyManagement.Application.UseCases.Receipts.Queries;

namespace FamilyManagement.Api.Endpoints.Receipts;

public class ListReceiptsRequest
{
    public string? Merchant { get; set; }
}

public class ListReceiptsEndpoint : Endpoint<ListReceiptsRequest, IReadOnlyList<ReceiptResponseDto>>
{
    private readonly ListReceiptsQueryHandler _handler;

    public ListReceiptsEndpoint(ListReceiptsQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/receipts");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "List all active receipts";
            s.Description = "Retrieves active receipts with optional filtering by merchant.";
        });
    }

    public override async Task HandleAsync(ListReceiptsRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new ListReceiptsQuery(req.Merchant), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
