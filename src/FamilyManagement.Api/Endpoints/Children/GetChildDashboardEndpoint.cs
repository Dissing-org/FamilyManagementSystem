using FastEndpoints;
using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Application.UseCases.Children.Queries;

namespace FamilyManagement.Api.Endpoints.Children;

/// <summary>
/// Request parameter model for retrieving a child dashboard.
/// </summary>
public class GetChildDashboardRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// Endpoint for retrieving a comprehensive child development dashboard.
/// </summary>
public class GetChildDashboardEndpoint : Endpoint<GetChildDashboardRequest, ChildDashboardDto>
{
    private readonly GetChildDashboardQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetChildDashboardEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler used to fetch the child dashboard.</param>
    public GetChildDashboardEndpoint(GetChildDashboardQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures HTTP route mapping, OpenAPI documentation metadata, and permissions.
    /// </summary>
    public override void Configure()
    {
        Get("/api/children/{id}/dashboard");
        Summary(s =>
        {
            s.Summary = "Get child development dashboard";
            s.Description = "Retrieves child profile, dynamic age, latest measurements, and upcoming/achieved milestones.";
        });
    }

    /// <summary>
    /// Handles the request to retrieve a child's development dashboard asynchronously.
    /// </summary>
    /// <param name="req">The request containing the child ID.</param>
    /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(GetChildDashboardRequest req, CancellationToken ct)
    {
        try
        {
            var result = await _handler.HandleAsync(new GetChildDashboardQuery(req.Id), ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
        }
    }
}
