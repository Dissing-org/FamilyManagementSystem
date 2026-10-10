using FastEndpoints;
using FamilyManagement.Application.Vehicles;

namespace FamilyManagement.Api.Endpoints.Vehicles;

public class GetMileageLogsRequest
{
    public Guid Id { get; set; }
}

public class GetMileageLogsEndpoint : Endpoint<GetMileageLogsRequest, List<MileageLogDto>>
{
    private readonly GetMileageLogsQueryHandler _handler;

    public GetMileageLogsEndpoint(GetMileageLogsQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/vehicles/{id}/mileage");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get mileage logs for a vehicle";
            s.Description = "Retrieves all odometer readings for a specific vehicle, ordered by date descending.";
        });
    }

    public override async Task HandleAsync(GetMileageLogsRequest req, CancellationToken ct)
    {
        var logs = await _handler.HandleAsync(new GetMileageLogsQuery(req.Id), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(logs));
    }
}
