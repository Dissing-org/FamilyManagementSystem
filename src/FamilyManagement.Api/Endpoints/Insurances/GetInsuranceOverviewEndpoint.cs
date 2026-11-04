using FastEndpoints;
using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Application.UseCases.Insurances.Queries;

namespace FamilyManagement.Api.Endpoints.Insurances;

public class GetInsuranceOverviewRequest
{
    public int DaysAhead { get; set; } = 90;
}

/// <summary>
/// Endpoint for retrieving an overview of family insurance policies, total costs, and upcoming renewals.
/// </summary>
public class GetInsuranceOverviewEndpoint : Endpoint<GetInsuranceOverviewRequest, InsuranceOverviewDto>
{
    private readonly GetInsuranceOverviewQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetInsuranceOverviewEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler for retrieving insurance overview data.</param>
    public GetInsuranceOverviewEndpoint(GetInsuranceOverviewQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route and OpenAPI metadata for retrieving the insurance overview.
    /// </summary>
    public override void Configure()
    {
        Get("/api/insurances/overview");
        Summary(s =>
        {
            s.Summary = "Get insurance overview and renewals";
            s.Description = "Calculates total normalized monthly and annual insurance costs, category breakdown, and upcoming renewals within specified days.";
        });
    }

    /// <summary>
    /// Handles the HTTP GET request to retrieve insurance overview metrics and upcoming renewals.
    /// </summary>
    /// <param name="req">The request parameters containing the lookup window in days.</param>
    /// <param name="ct">The cancellation token to observe.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task HandleAsync(GetInsuranceOverviewRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetInsuranceOverviewQuery(req.DaysAhead), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
