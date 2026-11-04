using FastEndpoints;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Queries;

namespace FamilyManagement.Api.Endpoints.Freezers;

/// <summary>
/// Endpoint that retrieves all freezers and their compartments with current item counts.
/// </summary>
public class GetFreezersEndpoint : EndpointWithoutRequest<IReadOnlyList<FreezerDto>>
{
    private readonly GetFreezersQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetFreezersEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler used to fetch all registered freezers.</param>
    public GetFreezersEndpoint(GetFreezersQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route mapping and OpenAPI specification for the endpoint.
    /// </summary>
    public override void Configure()
    {
        Get("/api/freezers");
        Summary(s =>
        {
            s.Summary = "Get all freezers";
            s.Description = "Retrieves all freezers and their compartments with item counts.";
        });
    }

    /// <summary>
    /// Handles the request to retrieve all freezers and their compartments.
    /// </summary>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetFreezersQuery(), ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
