using FastEndpoints;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Queries;

namespace FamilyManagement.Api.Endpoints.Freezers;

/// <summary>
/// Request parameters specifying the ID of the freezer to retrieve.
/// </summary>
public class GetFreezerByIdRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// Endpoint that retrieves a specific freezer with its compartments.
/// </summary>
public class GetFreezerByIdEndpoint : Endpoint<GetFreezerByIdRequest, FreezerDto>
{
    private readonly GetFreezerByIdQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetFreezerByIdEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler used to retrieve the freezer details.</param>
    public GetFreezerByIdEndpoint(GetFreezerByIdQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route mapping and OpenAPI specification for the endpoint.
    /// </summary>
    public override void Configure()
    {
        Get("/api/freezers/{id}");
        Summary(s =>
        {
            s.Summary = "Get freezer by ID";
            s.Description = "Retrieves a specific freezer with its compartments.";
        });
    }

    /// <summary>
    /// Handles the request to retrieve freezer details by ID.
    /// </summary>
    /// <param name="req">The query request containing the freezer ID.</param>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
