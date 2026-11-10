using FamilyManagement.Application.Interfaces;
using FamilyManagement.Application.UseCases.Gas.DTOs;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Gas.Commands;

public record SyncOkGasPriceCommand;

public class SyncOkGasPriceCommandHandler
{
    private readonly IGasPricingRepository _pricingRepository;
    private readonly IOkGasPriceScraper _scraper;

    public SyncOkGasPriceCommandHandler(
        IGasPricingRepository pricingRepository,
        IOkGasPriceScraper scraper)
    {
        _pricingRepository = pricingRepository;
        _scraper = scraper;
    }

    public async Task<GasPricingSettingsDto> HandleAsync(SyncOkGasPriceCommand command, CancellationToken ct = default)
    {
        var settings = await _pricingRepository.GetSettingsAsync(ct);
        var price = await _scraper.FetchCurrentCommodityPriceAsync(ct);

        if (price.HasValue && price.Value > 0)
        {
            settings.SyncOkPrice(price.Value);
            await _pricingRepository.SaveSettingsAsync(settings, ct);

            // Also snapshot for current month
            var now = DateTime.UtcNow;
            var snapshot = GasPriceSnapshot.Create(
                now.Year,
                now.Month,
                settings.CommodityPricePerM3,
                settings.DistributionTaxPerM3,
                settings.MonthlySubscription,
                isManuallyOverridden: false);

            await _pricingRepository.SaveSnapshotAsync(snapshot, ct);
        }

        return new GasPricingSettingsDto(
            settings.CommodityPricePerM3,
            settings.DistributionTaxPerM3,
            settings.MonthlySubscription,
            settings.TotalPricePerM3,
            settings.LastSyncedAt,
            settings.UpdatedAt);
    }
}
