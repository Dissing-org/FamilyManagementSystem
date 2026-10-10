using FastEndpoints;
using FamilyManagement.Application.Vehicles;

namespace FamilyManagement.Api.Endpoints.Vehicles;

public class GetVehicleSummaryRequest
{
    public Guid Id { get; set; }
}

public class GetVehicleSummaryEndpoint : Endpoint<GetVehicleSummaryRequest, VehicleSummaryDto>
{
    private readonly GetVehicleSummaryQueryHandler _handler;

    public GetVehicleSummaryEndpoint(GetVehicleSummaryQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/vehicles/{id}/summary");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get vehicle summary dashboard";
            s.Description = "Retrieves an aggregated summary including upcoming service interval countdowns (days and km), inspection due dates, maintenance alerts, and total maintenance costs.";
        });
    }

    public override async Task HandleAsync(GetVehicleSummaryRequest req, CancellationToken ct)
    {
        try
        {
            var summary = await _handler.HandleAsync(new GetVehicleSummaryQuery(req.Id), ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Ok(summary));
        }
        catch (KeyNotFoundException)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
        }
    }
}
