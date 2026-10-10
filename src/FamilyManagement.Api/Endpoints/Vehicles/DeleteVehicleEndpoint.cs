using FastEndpoints;
using FamilyManagement.Application.Vehicles;

namespace FamilyManagement.Api.Endpoints.Vehicles;

public class DeleteVehicleRequest
{
    public Guid Id { get; set; }
}

public class DeleteVehicleEndpoint : Endpoint<DeleteVehicleRequest>
{
    private readonly DeleteVehicleCommandHandler _handler;

    public DeleteVehicleEndpoint(DeleteVehicleCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Delete("/api/vehicles/{id}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Delete a vehicle";
            s.Description = "Removes a vehicle from the system.";
        });
    }

    public override async Task HandleAsync(DeleteVehicleRequest req, CancellationToken ct)
    {
        var deleted = await _handler.HandleAsync(new DeleteVehicleCommand(req.Id), ct);
        if (!deleted)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
            return;
        }

        await HttpContext.Response.SendResultAsync(TypedResults.NoContent());
    }
}
