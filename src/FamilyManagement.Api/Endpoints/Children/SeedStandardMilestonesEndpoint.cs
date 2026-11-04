using FastEndpoints;
using FamilyManagement.Application.UseCases.Children.Commands;

namespace FamilyManagement.Api.Endpoints.Children;

/// <summary>
/// Request parameter model for seeding standard milestones for a child.
/// </summary>
public class SeedStandardMilestonesRequest
{
    public Guid ChildId { get; set; }
}

/// <summary>
/// Response payload for standard milestone seeding.
/// </summary>
public class SeedStandardMilestonesResponse
{
    public int AddedCount { get; set; }
}

/// <summary>
/// Endpoint for seeding standard WHO/CDC developmental milestones for a child profile.
/// </summary>
public class SeedStandardMilestonesEndpoint : Endpoint<SeedStandardMilestonesRequest, SeedStandardMilestonesResponse>
{
    private readonly SeedStandardMilestonesCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="SeedStandardMilestonesEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler used to seed standard developmental milestones.</param>
    public SeedStandardMilestonesEndpoint(SeedStandardMilestonesCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures HTTP route mapping, OpenAPI documentation metadata, and permissions.
    /// </summary>
    public override void Configure()
    {
        Post("/api/children/{childId}/milestones/seed");
        Summary(s =>
        {
            s.Summary = "Seed standard developmental milestones";
            s.Description = "Populates WHO/CDC developmental expectations for a child (0 to 6 years).";
        });
    }

    /// <summary>
    /// Handles the request to seed standard developmental milestones asynchronously.
    /// </summary>
    /// <param name="req">The request containing the child ID.</param>
    /// <param name="ct">The cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(SeedStandardMilestonesRequest req, CancellationToken ct)
    {
        try
        {
            var addedCount = await _handler.HandleAsync(new SeedStandardMilestonesCommand(req.ChildId), ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Ok(new SeedStandardMilestonesResponse { AddedCount = addedCount }));
        }
        catch (KeyNotFoundException)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
        }
    }
}
