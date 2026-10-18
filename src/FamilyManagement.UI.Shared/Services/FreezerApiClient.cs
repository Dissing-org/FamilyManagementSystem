using System.Net.Http.Json;
using System.Text.Json;
using FamilyManagement.UI.Shared.Models;

namespace FamilyManagement.UI.Shared.Services;

public class FreezerApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IApiBaseUrlProvider _baseUrlProvider;

    public FreezerApiClient(HttpClient httpClient, IApiBaseUrlProvider baseUrlProvider)
    {
        _httpClient = httpClient;
        _baseUrlProvider = baseUrlProvider;
    }

    private string BuildUrl(string relativePath) =>
        _baseUrlProvider.ResolveUrl(_httpClient, relativePath);

    public async Task<FreezerDashboardDto?> GetDashboardAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<FreezerDashboardDto>(BuildUrl("api/freezers/dashboard"), ct);
            return response;
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<FreezerDto>> GetFreezersAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<FreezerDto>>(BuildUrl("api/freezers"), ct);
            return response ?? new List<FreezerDto>();
        }
        catch
        {
            return new List<FreezerDto>();
        }
    }

    public async Task<List<FreezerItemDto>> GetItemsAsync(
        Guid? compartmentId = null,
        string? category = null,
        string? freshness = null,
        bool? lowStockOnly = null,
        CancellationToken ct = default)
    {
        try
        {
            var query = new List<string>();
            if (compartmentId.HasValue) query.Add($"compartmentId={compartmentId.Value}");
            if (!string.IsNullOrWhiteSpace(category)) query.Add($"category={Uri.EscapeDataString(category)}");
            if (!string.IsNullOrWhiteSpace(freshness)) query.Add($"freshness={Uri.EscapeDataString(freshness)}");
            if (lowStockOnly == true) query.Add("lowStockOnly=true");

            var url = "api/freezer-items";
            if (query.Count > 0)
            {
                url += "?" + string.Join("&", query);
            }

            var response = await _httpClient.GetFromJsonAsync<List<FreezerItemDto>>(BuildUrl(url), ct);
            return response ?? new List<FreezerItemDto>();
        }
        catch
        {
            return new List<FreezerItemDto>();
        }
    }

    public async Task<(bool Success, string? Error, FreezerDto? Freezer)> CreateFreezerAsync(CreateFreezerRequest request, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(BuildUrl("api/freezers"), request, ct);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FreezerDto>(cancellationToken: ct);
                return (true, null, result);
            }

            var err = await TryExtractErrorMessageAsync(response, ct);
            return (false, err, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    public async Task<(bool Success, string? Error, FreezerDto? Freezer)> UpdateFreezerAsync(Guid id, UpdateFreezerRequest request, CancellationToken ct = default)
    {
        try
        {
            request.Id = id;
            var response = await _httpClient.PutAsJsonAsync(BuildUrl($"api/freezers/{id}"), request, ct);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FreezerDto>(cancellationToken: ct);
                return (true, null, result);
            }

            var err = await TryExtractErrorMessageAsync(response, ct);
            return (false, err, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    public async Task<bool> DeleteFreezerAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(BuildUrl($"api/freezers/{id}"), ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(bool Success, string? Error, FreezerCompartmentDto? Compartment)> CreateCompartmentAsync(Guid freezerId, CreateCompartmentRequest request, CancellationToken ct = default)
    {
        try
        {
            request.FreezerId = freezerId;
            var response = await _httpClient.PostAsJsonAsync(BuildUrl($"api/freezers/{freezerId}/compartments"), request, ct);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FreezerCompartmentDto>(cancellationToken: ct);
                return (true, null, result);
            }

            var err = await TryExtractErrorMessageAsync(response, ct);
            return (false, err, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    public async Task<bool> DeleteCompartmentAsync(Guid freezerId, Guid compartmentId, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(BuildUrl($"api/freezers/{freezerId}/compartments/{compartmentId}"), ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<(bool Success, string? Error, FreezerItemDto? Item)> CreateItemAsync(CreateFreezerItemRequest request, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(BuildUrl("api/freezer-items"), request, ct);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FreezerItemDto>(cancellationToken: ct);
                return (true, null, result);
            }

            var err = await TryExtractErrorMessageAsync(response, ct);
            return (false, err, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    public async Task<(bool Success, string? Error, FreezerItemDto? Item)> UpdateItemAsync(Guid id, UpdateFreezerItemRequest request, CancellationToken ct = default)
    {
        try
        {
            request.Id = id;
            var response = await _httpClient.PutAsJsonAsync(BuildUrl($"api/freezer-items/{id}"), request, ct);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FreezerItemDto>(cancellationToken: ct);
                return (true, null, result);
            }

            var err = await TryExtractErrorMessageAsync(response, ct);
            return (false, err, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    public async Task<(bool Success, string? Error, FreezerItemDto? Item)> ConsumeItemAsync(Guid id, ConsumeItemRequest request, CancellationToken ct = default)
    {
        try
        {
            request.Id = id;
            var response = await _httpClient.PostAsJsonAsync(BuildUrl($"api/freezer-items/{id}/consume"), request, ct);
            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<FreezerItemDto>(cancellationToken: ct);
                return (true, null, result);
            }

            var err = await TryExtractErrorMessageAsync(response, ct);
            return (false, err, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    public async Task<bool> DeleteItemAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(BuildUrl($"api/freezer-items/{id}"), ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    private static async Task<string> TryExtractErrorMessageAsync(HttpResponseMessage response, CancellationToken ct)
    {
        try
        {
            var content = await response.Content.ReadAsStringAsync(ct);
            if (string.IsNullOrWhiteSpace(content)) return response.ReasonPhrase ?? "Request failed";

            using var doc = JsonDocument.Parse(content);
            if (doc.RootElement.TryGetProperty("error", out var errorProp))
            {
                return errorProp.GetString() ?? response.ReasonPhrase ?? "Request failed";
            }

            if (doc.RootElement.TryGetProperty("errors", out var errorsProp))
            {
                var errList = new List<string>();
                foreach (var prop in errorsProp.EnumerateObject())
                {
                    foreach (var err in prop.Value.EnumerateArray())
                    {
                        var s = err.GetString();
                        if (!string.IsNullOrWhiteSpace(s)) errList.Add(s);
                    }
                }
                if (errList.Count > 0) return string.Join(" ", errList);
            }

            return content;
        }
        catch
        {
            return response.ReasonPhrase ?? "Request failed";
        }
    }
}
