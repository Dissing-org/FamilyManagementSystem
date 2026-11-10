using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using FamilyManagement.Application.UseCases.Gas.Commands;
using FamilyManagement.Application.UseCases.Gas.DTOs;

namespace FamilyManagement.Api.Endpoints.Gas;

public class UpdateGasPricingRequest
{
    public decimal CommodityPricePerM3 { get; set; }
    public decimal DistributionTaxPerM3 { get; set; }
    public decimal MonthlySubscription { get; set; }
}

public class UpdateGasPricingValidator : Validator<UpdateGasPricingRequest>
{
    public UpdateGasPricingValidator()
    {
        RuleFor(x => x.CommodityPricePerM3).GreaterThanOrEqualTo(0).WithMessage("Commodity price cannot be negative.");
        RuleFor(x => x.DistributionTaxPerM3).GreaterThanOrEqualTo(0).WithMessage("Distribution/tax cannot be negative.");
        RuleFor(x => x.MonthlySubscription).GreaterThanOrEqualTo(0).WithMessage("Subscription cannot be negative.");
    }
}

public class UpdateGasPricingEndpoint : Endpoint<UpdateGasPricingRequest, GasPricingSettingsDto>
{
    private readonly UpdateGasPricingSettingsCommandHandler _handler;

    public UpdateGasPricingEndpoint(UpdateGasPricingSettingsCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Put("/api/gas/pricing");
        Summary(s =>
        {
            s.Summary = "Update gas pricing configuration";
        });
    }

    public override async Task HandleAsync(UpdateGasPricingRequest req, CancellationToken ct)
    {
        try
        {
            var command = new UpdateGasPricingSettingsCommand(req.CommodityPricePerM3, req.DistributionTaxPerM3, req.MonthlySubscription);
            var result = await _handler.HandleAsync(command, ct);
            await HttpContext.Response.SendResultAsync(TypedResults.Ok(result));
        }
        catch (ArgumentException ex)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.BadRequest(new { error = ex.Message }));
        }
    }
}
