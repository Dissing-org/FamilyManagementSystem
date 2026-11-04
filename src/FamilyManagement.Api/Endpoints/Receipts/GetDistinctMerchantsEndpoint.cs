using FastEndpoints;
using FamilyManagement.Application.UseCases.Receipts.Queries;

namespace FamilyManagement.Api.Endpoints.Receipts;

/// <summary>
/// Endpoint for retrieving distinct merchant names from existing receipts.
/// </summary>
public class GetDistinctMerchantsEndpoint : EndpointWithoutRequest<IReadOnlyList<string>>
{
    private readonly GetDistinctMerchantsQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDistinctMerchantsEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler used to retrieve distinct merchant names.</param>
    public GetDistinctMerchantsEndpoint(GetDistinctMerchantsQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures HTTP route mapping, OpenAPI documentation metadata, and permissions.
    /// </summary>
    public override void Configure()
    {
        Get("/api/receipts/merchants");
        Summary(s =>
        {
            s.Summary = "Get distinct merchant names";
            s.Description = "Retrieves unique merchant names from existing receipts for autocomplete suggestions.";
        });
    }

    /// <summary>
    /// Handles the request to retrieve distinct merchant names asynchronously.
    /// </summary>
    /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetDistinctMerchantsQuery(), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
