using System.Net.Http.Json;
using FamilyManagement.UI.Shared.Models;

namespace FamilyManagement.UI.Shared.Services;

public interface IInsuranceApiClient
{
    Task<InsuranceOverviewModel?> GetOverviewAsync(int daysAhead = 90, CancellationToken ct = default);
    Task<List<InsurancePolicyViewModel>> GetPoliciesAsync(string? category = null, string? insurer = null, CancellationToken ct = default);
    Task<InsurancePolicyViewModel?> GetPolicyByIdAsync(Guid id, CancellationToken ct = default);
    Task<InsurancePolicyViewModel> CreatePolicyAsync(CreateOrUpdateInsurancePolicyModel model, CancellationToken ct = default);
    Task<InsurancePolicyViewModel> UpdatePolicyAsync(CreateOrUpdateInsurancePolicyModel model, CancellationToken ct = default);
    Task<bool> DeletePolicyAsync(Guid id, CancellationToken ct = default);
}

public class InsuranceApiClient : IInsuranceApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IApiBaseUrlProvider _baseUrlProvider;

    public InsuranceApiClient(HttpClient httpClient, IApiBaseUrlProvider baseUrlProvider)
    {
        _httpClient = httpClient;
        _baseUrlProvider = baseUrlProvider;
    }

    private string BuildUrl(string relativePath)
    {
        var baseUri = _baseUrlProvider.GetBaseUrl();
        if (string.IsNullOrWhiteSpace(baseUri))
        {
            return relativePath.TrimStart('/');
        }

        if (_httpClient.BaseAddress != null &&
            Uri.TryCreate(baseUri, UriKind.Absolute, out var parsedBase) &&
            string.Equals(parsedBase.Host, _httpClient.BaseAddress.Host, StringComparison.OrdinalIgnoreCase))
        {
            return relativePath.TrimStart('/');
        }

        return $"{baseUri.TrimEnd('/')}/{relativePath.TrimStart('/')}";
    }

    public async Task<InsuranceOverviewModel?> GetOverviewAsync(int daysAhead = 90, CancellationToken ct = default)
    {
        var url = BuildUrl($"api/insurances/overview?daysAhead={daysAhead}");
        return await _httpClient.GetFromJsonAsync<InsuranceOverviewModel>(url, ct);
    }

    public async Task<List<InsurancePolicyViewModel>> GetPoliciesAsync(string? category = null, string? insurer = null, CancellationToken ct = default)
    {
        var queryParams = new List<string>();
        if (!string.IsNullOrWhiteSpace(category))
        {
            queryParams.Add($"category={Uri.EscapeDataString(category)}");
        }
        if (!string.IsNullOrWhiteSpace(insurer))
        {
            queryParams.Add($"insurer={Uri.EscapeDataString(insurer)}");
        }

        var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        var url = BuildUrl($"api/insurances{queryString}");

        var response = await _httpClient.GetFromJsonAsync<List<InsurancePolicyViewModel>>(url, ct);
        return response ?? new List<InsurancePolicyViewModel>();
    }

    public async Task<InsurancePolicyViewModel?> GetPolicyByIdAsync(Guid id, CancellationToken ct = default)
    {
        var url = BuildUrl($"api/insurances/{id}");
        var response = await _httpClient.GetAsync(url, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<InsurancePolicyViewModel>(cancellationToken: ct);
    }

    public async Task<InsurancePolicyViewModel> CreatePolicyAsync(CreateOrUpdateInsurancePolicyModel model, CancellationToken ct = default)
    {
        var url = BuildUrl("api/insurances");
        var response = await _httpClient.PostAsJsonAsync(url, model, ct);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Failed to create insurance policy ({response.StatusCode}): {err}");
        }

        var created = await response.Content.ReadFromJsonAsync<InsurancePolicyViewModel>(cancellationToken: ct);
        return created ?? throw new InvalidOperationException("Failed to deserialize created insurance policy.");
    }

    public async Task<InsurancePolicyViewModel> UpdatePolicyAsync(CreateOrUpdateInsurancePolicyModel model, CancellationToken ct = default)
    {
        if (!model.Id.HasValue)
        {
            throw new ArgumentException("Policy ID must be set for update.", nameof(model));
        }

        var url = BuildUrl($"api/insurances/{model.Id.Value}");
        var response = await _httpClient.PutAsJsonAsync(url, model, ct);
        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Failed to update insurance policy ({response.StatusCode}): {err}");
        }

        var updated = await response.Content.ReadFromJsonAsync<InsurancePolicyViewModel>(cancellationToken: ct);
        return updated ?? throw new InvalidOperationException("Failed to deserialize updated insurance policy.");
    }

    public async Task<bool> DeletePolicyAsync(Guid id, CancellationToken ct = default)
    {
        var url = BuildUrl($"api/insurances/{id}");
        var response = await _httpClient.DeleteAsync(url, ct);
        return response.IsSuccessStatusCode;
    }
}
