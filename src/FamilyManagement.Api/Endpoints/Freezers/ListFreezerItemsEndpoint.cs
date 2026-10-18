using FastEndpoints;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Queries;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Api.Endpoints.Freezers;

public class ListFreezerItemsRequest
{
    public Guid? CompartmentId { get; set; }
    public FreezerCategory? Category { get; set; }
    public FreshnessStatus? Freshness { get; set; }
    public bool? LowStockOnly { get; set; }
}

public class ListFreezerItemsEndpoint : Endpoint<ListFreezerItemsRequest, IReadOnlyList<FreezerItemDto>>
{
    private readonly GetFreezerItemsQueryHandler _handler;

    public ListFreezerItemsEndpoint(GetFreezerItemsQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/freezer-items");
        Summary(s =>
        {
            s.Summary = "List freezer items";
            s.Description = "Retrieves freezer items with optional filtering by compartment, category, freshness, or low stock.";
        });
    }

    public override async Task HandleAsync(ListFreezerItemsRequest req, CancellationToken ct)
    {
        var query = new GetFreezerItemsQuery(req.CompartmentId, req.Category, req.Freshness, req.LowStockOnly);
        var result = await _handler.HandleAsync(query, ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
