using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace FamilyManagement.Infrastructure.Services;

public class OkGasPriceScraper : FamilyManagement.Application.Interfaces.IOkGasPriceScraper
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OkGasPriceScraper> _logger;
    private const string OkGasUrl = "https://www.ok.dk/privat/produkter/naturgas/priser";

    public OkGasPriceScraper(HttpClient httpClient, ILogger<OkGasPriceScraper> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<decimal?> FetchCurrentCommodityPriceAsync(CancellationToken ct = default)
    {
        try
        {
            var html = await _httpClient.GetStringAsync(OkGasUrl, ct);
            return ParsePriceFromHtml(html);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to scrape OK gas price from {Url}", OkGasUrl);
            return null;
        }
    }

    public static decimal? ParsePriceFromHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html)) return null;

        // Matches strings like "10,63 kr./m3", "10.63 kr./m3", "10,63 kr. pr. m3"
        var match = Regex.Match(html, @"(\d{1,3}[\.,]\d{2,4})\s*(?:kr\.?\s*\/\s*m3|kr\.\s*pr\.\s*m3|kr\.?\s*\/\s*m³)", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            var raw = match.Groups[1].Value.Replace(',', '.');
            if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
            {
                return price;
            }
        }
        return null;
    }
}
