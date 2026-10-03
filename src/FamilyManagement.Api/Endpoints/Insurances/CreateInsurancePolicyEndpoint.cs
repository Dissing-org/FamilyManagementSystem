using FastEndpoints;
using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Application.UseCases.Insurances.Commands;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Api.Endpoints.Insurances;

public class CreateInsurancePolicyRequest
{
    public string Insurer { get; set; } = string.Empty;
    public string? PolicyNumber { get; set; }
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public InsuranceCategory Category { get; set; } = InsuranceCategory.Other;
    public string InsuredParty { get; set; } = string.Empty;
    public decimal PremiumAmount { get; set; }
    public string Currency { get; set; } = "USD";
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public PaymentFrequency Frequency { get; set; } = PaymentFrequency.Monthly;
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime? RenewalDate { get; set; }
    public decimal? DeductibleAmount { get; set; }
    public string? Notes { get; set; }
}

public class CreateInsurancePolicyEndpoint : Endpoint<CreateInsurancePolicyRequest, InsurancePolicyResponseDto>
{
    private readonly CreateInsurancePolicyCommandHandler _handler;

    public CreateInsurancePolicyEndpoint(CreateInsurancePolicyCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/api/insurances");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Create a new insurance policy";
            s.Description = "Manually enters a new family insurance policy with premium details and renewal dates.";
        });
    }

    public override async Task HandleAsync(CreateInsurancePolicyRequest req, CancellationToken ct)
    {
        var command = new CreateInsurancePolicyCommand(
            req.Insurer,
            req.PolicyNumber,
            req.Category,
            req.InsuredParty,
            req.PremiumAmount,
            req.Currency,
            req.Frequency,
            req.StartDate,
            req.RenewalDate,
            req.DeductibleAmount,
            req.Notes);

        var result = await _handler.HandleAsync(command, ct);
        await HttpContext.Response.SendResultAsync(TypedResults.Created($"/api/insurances/{result.Id}", result));
    }
}
