using FastEndpoints;
using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Application.UseCases.Children.Queries;

namespace FamilyManagement.Api.Endpoints.Children;

public class GetChildDashboardRequest
{
    public Guid Id { get; set; }
}

public class GetChildDashboardEndpoint : Endpoint<GetChildDashboardRequest, ChildDashboardDto>
{
    private readonly GetChildDashboardQueryHandler _handler;

    public GetChildDashboardEndpoint(GetChildDashboardQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/children/{id}/dashboard");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get child development dashboard";
            s.Description = "Retrieves child profile, dynamic age, latest measurements, and upcoming/achieved milestones.";
        });
    }

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
