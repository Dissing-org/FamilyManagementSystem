using FastEndpoints;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Queries;

namespace FamilyManagement.Api.Endpoints.Freezers;

public class GetFreezerDashboardEndpoint : EndpointWithoutRequest<FreezerDashboardDto>
{
    private readonly GetFreezerDashboardQueryHandler _handler;

    public GetFreezerDashboardEndpoint(GetFreezerDashboardQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/freezers/dashboard");
        Summary(s =>
        {
            s.Summary = "Get freezer inventory dashboard";
            s.Description = "Retrieves freezers, counts, urgency alerts, and category distributions.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetFreezerDashboardQuery(), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
