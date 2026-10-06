using FastEndpoints;
using FamilyManagement.Application.UseCases.Children.Commands;

namespace FamilyManagement.Api.Endpoints.Children;

public class SeedStandardMilestonesRequest
{
    public Guid ChildId { get; set; }
}

public class SeedStandardMilestonesResponse
{
    public int AddedCount { get; set; }
}

public class SeedStandardMilestonesEndpoint : Endpoint<SeedStandardMilestonesRequest, SeedStandardMilestonesResponse>
{
    private readonly SeedStandardMilestonesCommandHandler _handler;

    public SeedStandardMilestonesEndpoint(SeedStandardMilestonesCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/api/children/{childId}/milestones/seed");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Seed standard developmental milestones";
            s.Description = "Populates WHO/CDC developmental expectations for a child (0 to 6 years).";
        });
    }

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
