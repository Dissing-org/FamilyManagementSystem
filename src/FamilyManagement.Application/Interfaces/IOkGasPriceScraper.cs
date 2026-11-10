namespace FamilyManagement.Application.Interfaces;

public interface IOkGasPriceScraper
{
    Task<decimal?> FetchCurrentCommodityPriceAsync(CancellationToken ct = default);
}
