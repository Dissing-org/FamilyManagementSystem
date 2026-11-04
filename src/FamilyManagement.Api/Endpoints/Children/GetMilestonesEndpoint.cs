using FastEndpoints;
using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Application.UseCases.Children.Queries;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Api.Endpoints.Children;

/// <summary>
/// Request parameter model for retrieving developmental milestones for a child.
/// </summary>
public class GetMilestonesRequest
{
    public Guid ChildId { get; set; }
    public MilestoneStatus? Status { get; set; }
    public MilestoneCategory? Category { get; set; }
}

/// <summary>
/// Endpoint for retrieving developmental milestones for a child, optionally filtered by status or category.
/// </summary>
public class GetMilestonesEndpoint : Endpoint<GetMilestonesRequest, List<ChildMilestoneDto>>
{
    private readonly GetMilestonesQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetMilestonesEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler used to fetch milestones.</param>
    public GetMilestonesEndpoint(GetMilestonesQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures HTTP route mapping, OpenAPI documentation metadata, and permissions.
    /// </summary>
    public override void Configure()
    {
        Get("/api/children/{childId}/milestones");
        Summary(s =>
        {
            s.Summary = "Get child milestones";
            s.Description = "Retrieves milestones filtered optionally by status (Expected, Achieved) or category.";
        });
    }

    /// <summary>
    /// Handles the request to retrieve developmental milestones asynchronously.
    /// </summary>
    /// <param name="req">The request containing the child ID and optional status/category filters.</param>
    /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(GetMilestonesRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetMilestonesQuery(req.ChildId, req.Status, req.Category), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
