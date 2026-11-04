using FastEndpoints;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Queries;

namespace FamilyManagement.Api.Endpoints.Freezers;

/// <summary>
/// Endpoint that retrieves dashboard summary statistics and urgency alerts across all freezers.
/// </summary>
public class GetFreezerDashboardEndpoint : EndpointWithoutRequest<FreezerDashboardDto>
{
    private readonly GetFreezerDashboardQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetFreezerDashboardEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler used to aggregate dashboard metrics.</param>
    public GetFreezerDashboardEndpoint(GetFreezerDashboardQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route mapping and OpenAPI specification for the endpoint.
    /// </summary>
    public override void Configure()
    {
        Get("/api/freezers/dashboard");
        Summary(s =>
        {
            s.Summary = "Get freezer inventory dashboard";
            s.Description = "Retrieves freezers, counts, urgency alerts, and category distributions.";
        });
    }

    /// <summary>
    /// Handles the request to retrieve freezer inventory dashboard metrics.
    /// </summary>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetFreezerDashboardQuery(), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
