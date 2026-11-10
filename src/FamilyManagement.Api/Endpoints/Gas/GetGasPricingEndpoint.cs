using FastEndpoints;
using Microsoft.AspNetCore.Http;
using FamilyManagement.Application.UseCases.Gas.DTOs;
using FamilyManagement.Application.UseCases.Gas.Queries;

namespace FamilyManagement.Api.Endpoints.Gas;

public class GetGasPricingEndpoint : EndpointWithoutRequest<GasPricingSettingsDto>
{
    private readonly GetGasPricingSettingsQueryHandler _handler;

    public GetGasPricingEndpoint(GetGasPricingSettingsQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/gas/pricing");
        Summary(s =>
        {
            s.Summary = "Get current gas pricing settings";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetGasPricingSettingsQuery(), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
