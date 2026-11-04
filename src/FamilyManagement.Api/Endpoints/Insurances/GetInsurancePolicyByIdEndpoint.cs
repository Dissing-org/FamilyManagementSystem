using FastEndpoints;
using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Application.UseCases.Insurances.Queries;

namespace FamilyManagement.Api.Endpoints.Insurances;

public class GetInsurancePolicyByIdRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// Endpoint for retrieving a specific insurance policy by its unique identifier.
/// </summary>
public class GetInsurancePolicyByIdEndpoint : Endpoint<GetInsurancePolicyByIdRequest, InsurancePolicyResponseDto>
{
    private readonly GetInsurancePolicyByIdQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetInsurancePolicyByIdEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler for retrieving an insurance policy by ID.</param>
    public GetInsurancePolicyByIdEndpoint(GetInsurancePolicyByIdQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route and OpenAPI metadata for retrieving an insurance policy by ID.
    /// </summary>
    public override void Configure()
    {
        Get("/api/insurances/{id}");
        Summary(s =>
        {
            s.Summary = "Get insurance policy by ID";
            s.Description = "Retrieves detailed information for a single insurance policy.";
        });
    }

    /// <summary>
    /// Handles the HTTP GET request to retrieve an insurance policy by its unique identifier.
    /// </summary>
    /// <param name="req">The request containing the unique identifier of the insurance policy.</param>
    /// <param name="ct">The cancellation token to observe.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task HandleAsync(GetInsurancePolicyByIdRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(new GetInsurancePolicyByIdQuery(req.Id), ct);
        if (result is null)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
            return;
        }

        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
