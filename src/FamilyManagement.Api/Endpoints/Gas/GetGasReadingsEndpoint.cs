using FastEndpoints;
using Microsoft.AspNetCore.Http;
using FamilyManagement.Application.UseCases.Gas.DTOs;
using FamilyManagement.Application.UseCases.Gas.Queries;

namespace FamilyManagement.Api.Endpoints.Gas;

public class GetGasReadingsEndpoint : EndpointWithoutRequest<List<GasReadingDto>>
{
    private readonly GetGasReadingsQueryHandler _handler;

    public GetGasReadingsEndpoint(GetGasReadingsQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/gas/readings");
        Summary(s =>
        {
            s.Summary = "Get all gas readings";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetGasReadingsQuery(), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
