using FastEndpoints;
using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Application.UseCases.Children.Queries;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Api.Endpoints.Children;

public class GetMilestonesRequest
{
    public Guid ChildId { get; set; }
    public MilestoneStatus? Status { get; set; }
    public MilestoneCategory? Category { get; set; }
}

public class GetMilestonesEndpoint : Endpoint<GetMilestonesRequest, List<ChildMilestoneDto>>
{
    private readonly GetMilestonesQueryHandler _handler;

    public GetMilestonesEndpoint(GetMilestonesQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/children/{childId}/milestones");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get child milestones";
            s.Description = "Retrieves milestones filtered optionally by status (Expected, Achieved) or category.";
        });
    }

    public override async Task HandleAsync(GetMilestonesRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetMilestonesQuery(req.ChildId, req.Status, req.Category), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
