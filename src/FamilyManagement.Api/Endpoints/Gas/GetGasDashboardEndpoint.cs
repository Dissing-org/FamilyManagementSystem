using FastEndpoints;
using Microsoft.AspNetCore.Http;
using FamilyManagement.Application.UseCases.Gas.DTOs;
using FamilyManagement.Application.UseCases.Gas.Queries;

namespace FamilyManagement.Api.Endpoints.Gas;

public class GetGasDashboardEndpoint : EndpointWithoutRequest<GasDashboardSummaryDto>
{
    private readonly GetGasDashboardQueryHandler _handler;

    public GetGasDashboardEndpoint(GetGasDashboardQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/gas/dashboard");
        Summary(s =>
        {
            s.Summary = "Get gas consumption dashboard summary";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetGasDashboardQuery(), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
