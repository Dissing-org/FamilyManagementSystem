using FastEndpoints;
using FamilyManagement.Application.DTOs;
using FamilyManagement.Application.UseCases.Receipts.Queries;

namespace FamilyManagement.Api.Endpoints.Receipts;

public class GetReceiptByIdRequest
{
    public Guid Id { get; set; }
}

public class GetReceiptByIdEndpoint : Endpoint<GetReceiptByIdRequest, ReceiptResponseDto>
{
    private readonly GetReceiptByIdQueryHandler _handler;

    public GetReceiptByIdEndpoint(GetReceiptByIdQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/receipts/{Id:guid}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get receipt by ID";
            s.Description = "Retrieves metadata and Google Drive file references for a specific receipt.";
        });
    }

    public override async Task HandleAsync(GetReceiptByIdRequest req, CancellationToken ct)
    {
        var receipt = await _handler.HandleAsync(new GetReceiptByIdQuery(req.Id), ct);
        if (receipt is null)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
            return;
        }

        await HttpContext.Response.SendResultAsync(TypedResults.Ok(receipt));
    }
}
