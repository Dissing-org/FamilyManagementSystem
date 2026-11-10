using FastEndpoints;
using Microsoft.AspNetCore.Http;
using FamilyManagement.Application.UseCases.Gas.Commands;
using FamilyManagement.Application.UseCases.Gas.DTOs;

namespace FamilyManagement.Api.Endpoints.Gas;

public class SyncOkGasPriceEndpoint : EndpointWithoutRequest<GasPricingSettingsDto>
{
    private readonly SyncOkGasPriceCommandHandler _handler;

    public SyncOkGasPriceEndpoint(SyncOkGasPriceCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/api/gas/pricing/sync");
        Summary(s =>
        {
            s.Summary = "Synchronize current gas price from OK";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new SyncOkGasPriceCommand(), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
