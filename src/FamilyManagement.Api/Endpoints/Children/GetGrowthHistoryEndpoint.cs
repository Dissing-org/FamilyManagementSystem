using FastEndpoints;
using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Application.UseCases.Children.Queries;

namespace FamilyManagement.Api.Endpoints.Children;

public class GetGrowthHistoryRequest
{
    public Guid ChildId { get; set; }
}

public class GetGrowthHistoryEndpoint : Endpoint<GetGrowthHistoryRequest, List<GrowthMeasurementDto>>
{
    private readonly GetGrowthHistoryQueryHandler _handler;

    public GetGrowthHistoryEndpoint(GetGrowthHistoryQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/children/{childId}/growth");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get child growth measurement history";
            s.Description = "Retrieves chronological biometric measurements for height, weight, and head circumference.";
        });
    }

    public override async Task HandleAsync(GetGrowthHistoryRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetGrowthHistoryQuery(req.ChildId), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
