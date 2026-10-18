using FastEndpoints;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Queries;

namespace FamilyManagement.Api.Endpoints.Freezers;

public class GetFreezerByIdRequest
{
    public Guid Id { get; set; }
}

public class GetFreezerByIdEndpoint : Endpoint<GetFreezerByIdRequest, FreezerDto>
{
    private readonly GetFreezerByIdQueryHandler _handler;

    public GetFreezerByIdEndpoint(GetFreezerByIdQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/freezers/{id}");
        Summary(s =>
        {
            s.Summary = "Get freezer by ID";
            s.Description = "Retrieves a specific freezer with its compartments.";
        });
    }

    public override async Task HandleAsync(GetFreezerByIdRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetFreezerByIdQuery(req.Id), ct);
        if (result is null)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound(new { error = $"Freezer '{req.Id}' not found." }));
            return;
        }

        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
