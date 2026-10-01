using FastEndpoints;
using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Application.UseCases.Insurances.Queries;

namespace FamilyManagement.Api.Endpoints.Insurances;

public class GetInsurancePolicyByIdRequest
{
    public Guid Id { get; set; }
}

public class GetInsurancePolicyByIdEndpoint : Endpoint<GetInsurancePolicyByIdRequest, InsurancePolicyResponseDto>
{
    private readonly GetInsurancePolicyByIdQueryHandler _handler;

    public GetInsurancePolicyByIdEndpoint(GetInsurancePolicyByIdQueryHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Get("/api/insurances/{id}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Get insurance policy by ID";
            s.Description = "Retrieves detailed information for a single insurance policy.";
        });
    }

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
