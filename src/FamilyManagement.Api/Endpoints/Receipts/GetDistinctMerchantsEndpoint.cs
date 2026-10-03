using FastEndpoints;
using FamilyManagement.Application.UseCases.Receipts.Queries;

namespace FamilyManagement.Api.Endpoints.Receipts;

public class GetDistinctMerchantsEndpoint : EndpointWithoutRequest<IReadOnlyList<string>>
{
    private readonly GetDistinctMerchantsQueryHandler _handler;

    public GetDistinctMerchantsEndpoint(GetDistinctMerchantsQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/receipts/merchants");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get distinct merchant names";
            s.Description = "Retrieves unique merchant names from existing receipts for autocomplete suggestions.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetDistinctMerchantsQuery(), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
