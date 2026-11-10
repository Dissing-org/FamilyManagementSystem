using FastEndpoints;
using Microsoft.AspNetCore.Http;
using FamilyManagement.Application.UseCases.Gas.Commands;

namespace FamilyManagement.Api.Endpoints.Gas;

public class DeleteGasReadingRequest
{
    public Guid Id { get; set; }
}

public class DeleteGasReadingEndpoint : Endpoint<DeleteGasReadingRequest>
{
    private readonly DeleteGasReadingCommandHandler _handler;

    public DeleteGasReadingEndpoint(DeleteGasReadingCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Delete("/api/gas/readings/{id}");
        Summary(s =>
        {
            s.Summary = "Delete a gas meter reading";
        });
    }

    public override async Task HandleAsync(DeleteGasReadingRequest req, CancellationToken ct)
    {
        try
        {
            await _handler.HandleAsync(new DeleteGasReadingCommand(req.Id), ct);
            await HttpContext.Response.SendResultAsync(TypedResults.NoContent());
        }
        catch (KeyNotFoundException)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
        }
    }
}
