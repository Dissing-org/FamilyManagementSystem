using FastEndpoints;
using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Application.UseCases.Children.Queries;

namespace FamilyManagement.Api.Endpoints.Children;

/// <summary>
/// Request parameter model for retrieving growth measurement history for a child.
/// </summary>
public class GetGrowthHistoryRequest
{
    public Guid ChildId { get; set; }
}

/// <summary>
/// Endpoint for retrieving chronological biometric growth measurements for a child.
/// </summary>
public class GetGrowthHistoryEndpoint : Endpoint<GetGrowthHistoryRequest, List<GrowthMeasurementDto>>
{
    private readonly GetGrowthHistoryQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetGrowthHistoryEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler used to fetch growth history.</param>
    public GetGrowthHistoryEndpoint(GetGrowthHistoryQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures HTTP route mapping, OpenAPI documentation metadata, and permissions.
    /// </summary>
    public override void Configure()
    {
        Get("/api/children/{childId}/growth");
        Summary(s =>
        {
            s.Summary = "Get child growth measurement history";
            s.Description = "Retrieves chronological biometric measurements for height, weight, and head circumference.";
        });
    }

    /// <summary>
    /// Handles the request to retrieve growth measurement history asynchronously.
    /// </summary>
    /// <param name="req">The request containing the child ID.</param>
    /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(GetGrowthHistoryRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetGrowthHistoryQuery(req.ChildId), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
