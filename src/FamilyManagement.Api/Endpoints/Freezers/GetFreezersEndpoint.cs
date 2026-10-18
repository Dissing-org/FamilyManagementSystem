using FastEndpoints;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Queries;

namespace FamilyManagement.Api.Endpoints.Freezers;

public class GetFreezersEndpoint : EndpointWithoutRequest<IReadOnlyList<FreezerDto>>
{
    private readonly GetFreezersQueryHandler _handler;

    public GetFreezersEndpoint(GetFreezersQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/freezers");
        Summary(s =>
        {
            s.Summary = "Get all freezers";
            s.Description = "Retrieves all freezers and their compartments with item counts.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetFreezersQuery(), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
