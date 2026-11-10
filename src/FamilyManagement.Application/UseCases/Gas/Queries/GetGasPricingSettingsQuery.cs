using FamilyManagement.Application.UseCases.Gas.DTOs;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Gas.Queries;

public record GetGasPricingSettingsQuery;

public class GetGasPricingSettingsQueryHandler
{
    private readonly IGasPricingRepository _pricingRepository;

    public GetGasPricingSettingsQueryHandler(IGasPricingRepository pricingRepository)
    {
        _pricingRepository = pricingRepository;
    }

    public async Task<GasPricingSettingsDto> HandleAsync(GetGasPricingSettingsQuery query, CancellationToken ct = default)
    {
        var settings = await _pricingRepository.GetSettingsAsync(ct);
        return new GasPricingSettingsDto(
            settings.CommodityPricePerM3,
            settings.DistributionTaxPerM3,
            settings.MonthlySubscription,
            settings.TotalPricePerM3,
            settings.LastSyncedAt,
            settings.UpdatedAt);
    }
}
