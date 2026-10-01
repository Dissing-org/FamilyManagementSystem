using FastEndpoints;
using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Application.UseCases.Insurances.Queries;

namespace FamilyManagement.Api.Endpoints.Insurances;

public class GetInsuranceOverviewRequest
{
    public int DaysAhead { get; set; } = 90;
}

public class GetInsuranceOverviewEndpoint : Endpoint<GetInsuranceOverviewRequest, InsuranceOverviewDto>
{
    private readonly GetInsuranceOverviewQueryHandler _handler;

    public GetInsuranceOverviewEndpoint(GetInsuranceOverviewQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/insurances/overview");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get insurance overview and renewals";
            s.Description = "Calculates total normalized monthly and annual insurance costs, category breakdown, and upcoming renewals within specified days.";
        });
    }

    public override async Task HandleAsync(GetInsuranceOverviewRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetInsuranceOverviewQuery(req.DaysAhead), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
