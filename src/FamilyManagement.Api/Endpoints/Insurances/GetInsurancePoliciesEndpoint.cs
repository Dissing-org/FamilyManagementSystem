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

public class GetInsurancePoliciesEndpoint : Endpoint<GetInsurancePoliciesRequest, List<InsurancePolicyResponseDto>>
{
    private readonly GetInsurancePoliciesQueryHandler _handler;

    public GetInsurancePoliciesEndpoint(GetInsurancePoliciesQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/insurances");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "List insurance policies";
            s.Description = "Retrieves all family insurance policies with optional filters for category, status, and insurer.";
        });
    }

    public override async Task HandleAsync(GetInsurancePoliciesRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(
            new GetInsurancePoliciesQuery(req.Category, req.Status, req.Insurer),
            ct);

        await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
    }
}
