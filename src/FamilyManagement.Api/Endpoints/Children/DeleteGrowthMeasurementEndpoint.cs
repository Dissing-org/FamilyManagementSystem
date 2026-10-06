using FastEndpoints;
using FamilyManagement.Application.UseCases.Children.Commands;

namespace FamilyManagement.Api.Endpoints.Children;

public class DeleteGrowthMeasurementRequest
{
    public Guid Id { get; set; }
}

public class DeleteGrowthMeasurementEndpoint : Endpoint<DeleteGrowthMeasurementRequest>
{
    private readonly DeleteGrowthMeasurementCommandHandler _handler;

    public DeleteGrowthMeasurementEndpoint(DeleteGrowthMeasurementCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Delete("/api/children/growth/{id}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Delete a growth measurement";
            s.Description = "Deletes a previously recorded height/weight measurement.";
        });
    }

    public override async Task HandleAsync(DeleteGrowthMeasurementRequest req, CancellationToken ct)
    {
        var deleted = await _handler.HandleAsync(new DeleteGrowthMeasurementCommand(req.Id), ct);
        if (deleted)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NoContent());
        }
        else
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
        }
    }
}
