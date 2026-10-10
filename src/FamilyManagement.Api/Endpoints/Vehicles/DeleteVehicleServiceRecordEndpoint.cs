using FastEndpoints;
using FamilyManagement.Application.Vehicles;

namespace FamilyManagement.Api.Endpoints.Vehicles;

public class DeleteVehicleServiceRecordRequest
{
    public Guid Id { get; set; }
    public Guid ServiceId { get; set; }
}

public class DeleteVehicleServiceRecordEndpoint : Endpoint<DeleteVehicleServiceRecordRequest>
{
    private readonly DeleteVehicleServiceRecordCommandHandler _handler;

    public DeleteVehicleServiceRecordEndpoint(DeleteVehicleServiceRecordCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Delete("/api/vehicles/{id}/services/{serviceId}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Delete a service record";
            s.Description = "Removes a specific service or repair entry from the vehicle maintenance log.";
        });
    }

    public override async Task HandleAsync(DeleteVehicleServiceRecordRequest req, CancellationToken ct)
    {
        var deleted = await _handler.HandleAsync(new DeleteVehicleServiceRecordCommand(req.ServiceId), ct);
        if (!deleted)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
            return;
        }

        await HttpContext.Response.SendResultAsync(TypedResults.NoContent());
    }
}
