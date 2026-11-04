using FastEndpoints;
using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Application.UseCases.Insurances.Queries;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Api.Endpoints.Insurances;

public class GetInsurancePoliciesRequest
{
    public InsuranceCategory? Category { get; set; }
    public InsurancePolicyStatus? Status { get; set; }
    public string? Insurer { get; set; }
}

/// <summary>
/// Endpoint for retrieving a list of family insurance policies with optional filters.
/// </summary>
public class GetInsurancePoliciesEndpoint : Endpoint<GetInsurancePoliciesRequest, List<InsurancePolicyResponseDto>>
{
    private readonly GetInsurancePoliciesQueryHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetInsurancePoliciesEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The query handler for listing insurance policies.</param>
    public GetInsurancePoliciesEndpoint(GetInsurancePoliciesQueryHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route and OpenAPI metadata for listing insurance policies.
    /// </summary>
    public override void Configure()
    {
        Get("/api/insurances");
        Summary(s =>
        {
            s.Summary = "List insurance policies";
            s.Description = "Retrieves all family insurance policies with optional filters for category, status, and insurer.";
        });
    }

    /// <summary>
    /// Handles the HTTP GET request to retrieve family insurance policies matching optional filters.
    /// </summary>
    /// <param name="req">The request query parameters for filtering policies.</param>
    /// <param name="ct">The cancellation token to observe.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task HandleAsync(GetInsurancePoliciesRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(
            new GetInsurancePoliciesQuery(req.Category, req.Status, req.Insurer),
            ct);

        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
