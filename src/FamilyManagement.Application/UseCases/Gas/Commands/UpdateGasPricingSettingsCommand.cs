using FamilyManagement.Application.UseCases.Gas.DTOs;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Gas.Commands;

public record UpdateGasPricingSettingsCommand(
    decimal CommodityPricePerM3,
    decimal DistributionTaxPerM3,
    decimal MonthlySubscription);

public class UpdateGasPricingSettingsCommandHandler
{
    private readonly IGasPricingRepository _pricingRepository;

    public UpdateGasPricingSettingsCommandHandler(IGasPricingRepository pricingRepository)
    {
        _pricingRepository = pricingRepository;
    }

    public async Task<GasPricingSettingsDto> HandleAsync(UpdateGasPricingSettingsCommand command, CancellationToken ct = default)
    {
        var settings = await _pricingRepository.GetSettingsAsync(ct);
        settings.Update(command.CommodityPricePerM3, command.DistributionTaxPerM3, command.MonthlySubscription);
        await _pricingRepository.SaveSettingsAsync(settings, ct);

        return new GasPricingSettingsDto(
            settings.CommodityPricePerM3,
            settings.DistributionTaxPerM3,
            settings.MonthlySubscription,
            settings.TotalPricePerM3,
            settings.LastSyncedAt,
            settings.UpdatedAt);
    }
}
