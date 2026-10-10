using FastEndpoints;
using FamilyManagement.Application.Vehicles;

namespace FamilyManagement.Api.Endpoints.Vehicles;

public class GetServiceRecordsRequest
{
    public Guid Id { get; set; }
}

public class GetServiceRecordsEndpoint : Endpoint<GetServiceRecordsRequest, List<VehicleServiceRecordDto>>
{
    private readonly GetServiceRecordsQueryHandler _handler;

    public GetServiceRecordsEndpoint(GetServiceRecordsQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/vehicles/{id}/services");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get service history for a vehicle";
            s.Description = "Retrieves all recorded maintenance and repairs for a vehicle, ordered by date descending.";
        });
    }

    public override async Task HandleAsync(GetServiceRecordsRequest req, CancellationToken ct)
    {
        var records = await _handler.HandleAsync(new GetServiceRecordsQuery(req.Id), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(records));
    }
}
