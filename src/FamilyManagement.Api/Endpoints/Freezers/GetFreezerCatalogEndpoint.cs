using FastEndpoints;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Queries;

namespace FamilyManagement.Api.Endpoints.Freezers;

/// <summary>
/// Endpoint that retrieves all persistent freezer catalog item suggestions.
/// </summary>
public class GetFreezerCatalogEndpoint : EndpointWithoutRequest<IReadOnlyList<FreezerCatalogItemDto>>
{
    private readonly GetFreezerCatalogQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetFreezerCatalogEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler used to fetch freezer catalog items.</param>
    public GetFreezerCatalogEndpoint(GetFreezerCatalogQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route mapping and OpenAPI specification for the endpoint.
    /// </summary>
    public override void Configure()
    {
        Get("/api/freezers/catalog");
        Summary(s =>
        {
            s.Summary = "Get freezer item catalog";
            s.Description = "Returns all persistent freezer catalog suggestions ordered alphabetically.";
        });
    }

    /// <summary>
    /// Handles the request to retrieve all saved freezer catalog suggestions.
    /// </summary>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetFreezerCatalogQuery(), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
