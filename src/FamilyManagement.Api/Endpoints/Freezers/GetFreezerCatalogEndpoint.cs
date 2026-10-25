using FastEndpoints;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Queries;

namespace FamilyManagement.Api.Endpoints.Freezers;

public class GetFreezerCatalogEndpoint : EndpointWithoutRequest<IReadOnlyList<FreezerCatalogItemDto>>
{
    private readonly GetFreezerCatalogQueryHandler _handler;

    public GetFreezerCatalogEndpoint(GetFreezerCatalogQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/freezers/catalog");
        Summary(s =>
        {
            s.Summary = "Get freezer item catalog";
            s.Description = "Returns all persistent freezer catalog suggestions ordered alphabetically.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetFreezerCatalogQuery(), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
