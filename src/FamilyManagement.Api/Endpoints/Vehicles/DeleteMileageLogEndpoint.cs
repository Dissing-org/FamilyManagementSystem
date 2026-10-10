using FastEndpoints;
using FamilyManagement.Application.Vehicles;

namespace FamilyManagement.Api.Endpoints.Vehicles;

public class DeleteMileageLogRequest
{
    public Guid Id { get; set; }
    public Guid LogId { get; set; }
}

public class DeleteMileageLogEndpoint : Endpoint<DeleteMileageLogRequest>
{
    private readonly DeleteMileageLogCommandHandler _handler;

    public DeleteMileageLogEndpoint(DeleteMileageLogCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Delete("/api/vehicles/{id}/mileage/{logId}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Delete a mileage log entry";
            s.Description = "Removes a specific mileage log reading from the vehicle history.";
        });
    }

    public override async Task HandleAsync(DeleteMileageLogRequest req, CancellationToken ct)
    {
        var deleted = await _handler.HandleAsync(new DeleteMileageLogCommand(req.Id, req.LogId), ct);
        if (!deleted)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
            return;
        }

        await HttpContext.Response.SendResultAsync(TypedResults.NoContent());
    }
}
