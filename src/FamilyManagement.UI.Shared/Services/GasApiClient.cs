using System.Net.Http.Json;
using FamilyManagement.UI.Shared.Models;

namespace FamilyManagement.UI.Shared.Services;

public class GasApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IApiBaseUrlProvider _baseUrlProvider;

    public GasApiClient(HttpClient httpClient, IApiBaseUrlProvider baseUrlProvider)
    {
        _httpClient = httpClient;
        _baseUrlProvider = baseUrlProvider;
    }

    private string BuildUrl(string relativePath) =>
        _baseUrlProvider.ResolveUrl(_httpClient, relativePath);

    public async Task<GasDashboardSummaryDto?> GetDashboardSummaryAsync(CancellationToken ct = default)
    {
        try
        {
            var url = BuildUrl("api/gas/dashboard");
            return await _httpClient.GetFromJsonAsync<GasDashboardSummaryDto>(url, ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<GasReadingDto>> GetReadingsAsync(CancellationToken ct = default)
    {
        try
        {
            var url = BuildUrl("api/gas/readings");
            var res = await _httpClient.GetFromJsonAsync<List<GasReadingDto>>(url, ct);
            return res ?? new List<GasReadingDto>();
        }
        catch
        {
            return new List<GasReadingDto>();
        }
    }

    public async Task<GasReadingDto?> LogReadingAsync(LogGasReadingRequest request, CancellationToken ct = default)
    {
        try
        {
            var url = BuildUrl("api/gas/readings");
            var response = await _httpClient.PostAsJsonAsync(url, request, ct);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<GasReadingDto>(cancellationToken: ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task<GasReadingDto?> UpdateReadingAsync(UpdateGasReadingRequest request, CancellationToken ct = default)
    {
        try
        {
            var url = BuildUrl($"api/gas/readings/{request.Id}");
            var response = await _httpClient.PutAsJsonAsync(url, request, ct);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<GasReadingDto>(cancellationToken: ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> DeleteReadingAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var url = BuildUrl($"api/gas/readings/{id}");
            var response = await _httpClient.DeleteAsync(url, ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<GasPricingSettingsDto?> GetPricingSettingsAsync(CancellationToken ct = default)
    {
        try
        {
            var url = BuildUrl("api/gas/pricing");
            return await _httpClient.GetFromJsonAsync<GasPricingSettingsDto>(url, ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task<GasPricingSettingsDto?> UpdatePricingSettingsAsync(UpdateGasPricingRequest request, CancellationToken ct = default)
    {
        try
        {
            var url = BuildUrl("api/gas/pricing");
            var response = await _httpClient.PutAsJsonAsync(url, request, ct);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<GasPricingSettingsDto>(cancellationToken: ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task<GasPricingSettingsDto?> SyncOkPriceAsync(CancellationToken ct = default)
    {
        try
        {
            var url = BuildUrl("api/gas/pricing/sync");
            var response = await _httpClient.PostAsync(url, null, ct);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<GasPricingSettingsDto>(cancellationToken: ct);
        }
        catch
        {
            return null;
        }
    }
}
