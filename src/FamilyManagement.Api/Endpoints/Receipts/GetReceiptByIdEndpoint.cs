using FastEndpoints;
using FamilyManagement.Application.DTOs;
using FamilyManagement.Application.UseCases.Receipts.Queries;

namespace FamilyManagement.Api.Endpoints.Receipts;

/// <summary>
/// Request parameter model for retrieving a receipt by its unique identifier.
/// </summary>
public class GetReceiptByIdRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// Endpoint for retrieving metadata and storage references for a specific receipt.
/// </summary>
public class GetReceiptByIdEndpoint : Endpoint<GetReceiptByIdRequest, ReceiptResponseDto>
{
    private readonly GetReceiptByIdQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetReceiptByIdEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler used to retrieve receipt details.</param>
    public GetReceiptByIdEndpoint(GetReceiptByIdQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures HTTP route mapping, OpenAPI documentation metadata, and permissions.
    /// </summary>
    public override void Configure()
    {
        Get("/api/receipts/{Id:guid}");
        Summary(s =>
        {
            s.Summary = "Get receipt by ID";
            s.Description = "Retrieves metadata and Google Drive file references for a specific receipt.";
        });
    }

    /// <summary>
    /// Handles the request to retrieve receipt details by ID asynchronously.
    /// </summary>
    /// <param name="req">The request containing the receipt ID.</param>
    /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
