using FastEndpoints;
using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Application.UseCases.Insurances.Commands;
using FamilyManagement.Domain.Enums;

namespace FamilyManagement.Api.Endpoints.Insurances;

public class UpdateInsurancePolicyRequest
{
    public Guid Id { get; set; }
    public string Insurer { get; set; } = string.Empty;
    public string? PolicyNumber { get; set; }
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public InsuranceCategory Category { get; set; }
    public string InsuredParty { get; set; } = string.Empty;
    public decimal PremiumAmount { get; set; }
    public string Currency { get; set; } = "USD";
    [System.Text.Json.Serialization.JsonConverter(typeof(System.Text.Json.Serialization.JsonStringEnumConverter))]
    public PaymentFrequency Frequency { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? RenewalDate { get; set; }
    public decimal? DeductibleAmount { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Endpoint for updating an existing insurance policy.
/// </summary>
public class UpdateInsurancePolicyEndpoint : Endpoint<UpdateInsurancePolicyRequest, InsurancePolicyResponseDto>
{
    private readonly UpdateInsurancePolicyCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateInsurancePolicyEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler for updating insurance policies.</param>
    public UpdateInsurancePolicyEndpoint(UpdateInsurancePolicyCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route and OpenAPI metadata for updating an insurance policy.
    /// </summary>
    public override void Configure()
    {
        Put("/api/insurances/{id}");
        Summary(s =>
        {
            s.Summary = "Update insurance policy details";
            s.Description = "Updates coverage details, policy numbers, premium amounts, or renewal dates.";
        });
    }

    /// <summary>
    /// Handles the HTTP PUT request to update an existing insurance policy.
    /// </summary>
    /// <param name="req">The request payload containing updated insurance policy details.</param>
    /// <param name="ct">The cancellation token to observe.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task HandleAsync(UpdateInsurancePolicyRequest req, CancellationToken ct)
    {
        try
        {
            var command = new UpdateInsurancePolicyCommand(
                req.Id,
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
            await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
        }
        catch (KeyNotFoundException)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
        }
    }
}
