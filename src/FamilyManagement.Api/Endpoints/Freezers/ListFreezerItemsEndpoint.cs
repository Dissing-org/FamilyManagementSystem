using FastEndpoints;
using System.Text.Json.Serialization;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Queries;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Api.Endpoints.Freezers;

/// <summary>
/// Request parameters for querying and filtering stored freezer items.
/// </summary>
public class ListFreezerItemsRequest
{
    public Guid? CompartmentId { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FreezerCategory? Category { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public FreshnessStatus? Freshness { get; set; }

    public bool? LowStockOnly { get; set; }
}

/// <summary>
/// Endpoint that lists freezer items with optional filtering by compartment, category, freshness, or low stock.
/// </summary>
public class ListFreezerItemsEndpoint : Endpoint<ListFreezerItemsRequest, IReadOnlyList<FreezerItemDto>>
{
    private readonly GetFreezerItemsQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListFreezerItemsEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler used to filter and retrieve freezer items.</param>
    public ListFreezerItemsEndpoint(GetFreezerItemsQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route mapping and OpenAPI specification for the endpoint.
    /// </summary>
    public override void Configure()
    {
        Get("/api/freezer-items");
        Summary(s =>
        {
            s.Summary = "List freezer items";
            s.Description = "Retrieves freezer items with optional filtering by compartment, category, freshness, or low stock.";
        });
    }

    /// <summary>
    /// Handles the request to retrieve freezer items matching filter criteria.
    /// </summary>
    /// <param name="req">The query filter parameters.</param>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(ListFreezerItemsRequest req, CancellationToken ct)
    {
        var query = new GetFreezerItemsQuery(req.CompartmentId, req.Category, req.Freshness, req.LowStockOnly);
        var result = await _handler.HandleAsync(query, ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
