using System.Net.Http.Json;
using System.Text.Json;
using FamilyManagement.UI.Shared.Models;

namespace FamilyManagement.UI.Shared.Services;

public class VehicleApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IApiBaseUrlProvider _baseUrlProvider;

    public VehicleApiClient(HttpClient httpClient, IApiBaseUrlProvider baseUrlProvider)
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

    public async Task<List<VehicleDto>> GetVehiclesAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<VehicleDto>>(BuildUrl("api/vehicles"), ct);
            return response ?? new List<VehicleDto>();
        }
        catch
        {
            return new List<VehicleDto>();
        }
    }

    public async Task<VehicleDto?> GetVehicleByIdAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var url = BuildUrl($"api/vehicles/{id}");
            var response = await _httpClient.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<VehicleDto>(cancellationToken: ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task<VehicleSummaryDto?> GetVehicleSummaryAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var url = BuildUrl($"api/vehicles/{id}/summary");
            var response = await _httpClient.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<VehicleSummaryDto>(cancellationToken: ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task<(bool Success, string? Error, VehicleDto? Vehicle)> CreateVehicleAsync(CreateVehicleRequest request, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(BuildUrl("api/vehicles"), request, ct);
            if (response.IsSuccessStatusCode)
            {
                var vehicle = await response.Content.ReadFromJsonAsync<VehicleDto>(cancellationToken: ct);
                return (true, null, vehicle);
            }

            var err = await TryExtractErrorMessageAsync(response, ct);
            return (false, err, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    public async Task<(bool Success, string? Error, VehicleDto? Vehicle)> UpdateVehicleAsync(Guid id, UpdateVehicleRequest request, CancellationToken ct = default)
    {
        try
        {
            request.Id = id;
            var response = await _httpClient.PutAsJsonAsync(BuildUrl($"api/vehicles/{id}"), request, ct);
            if (response.IsSuccessStatusCode)
            {
                var vehicle = await response.Content.ReadFromJsonAsync<VehicleDto>(cancellationToken: ct);
                return (true, null, vehicle);
            }

            var err = await TryExtractErrorMessageAsync(response, ct);
            return (false, err, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    public async Task<bool> DeleteVehicleAsync(Guid id, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(BuildUrl($"api/vehicles/{id}"), ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<MileageLogDto>> GetMileageLogsAsync(Guid vehicleId, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<MileageLogDto>>(BuildUrl($"api/vehicles/{vehicleId}/mileage"), ct);
            return response ?? new List<MileageLogDto>();
        }
        catch
        {
            return new List<MileageLogDto>();
        }
    }

    public async Task<(bool Success, string? Error, MileageLogDto? Log)> LogMileageAsync(Guid vehicleId, LogMileageRequest request, CancellationToken ct = default)
    {
        try
        {
            request.Id = vehicleId;
            var response = await _httpClient.PostAsJsonAsync(BuildUrl($"api/vehicles/{vehicleId}/mileage"), request, ct);
            if (response.IsSuccessStatusCode)
            {
                var log = await response.Content.ReadFromJsonAsync<MileageLogDto>(cancellationToken: ct);
                return (true, null, log);
            }

            var err = await TryExtractErrorMessageAsync(response, ct);
            return (false, err, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    public async Task<bool> DeleteMileageLogAsync(Guid vehicleId, Guid logId, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(BuildUrl($"api/vehicles/{vehicleId}/mileage/{logId}"), ct);
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<VehicleServiceRecordDto>> GetServiceRecordsAsync(Guid vehicleId, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<VehicleServiceRecordDto>>(BuildUrl($"api/vehicles/{vehicleId}/services"), ct);
            return response ?? new List<VehicleServiceRecordDto>();
        }
        catch
        {
            return new List<VehicleServiceRecordDto>();
        }
    }

    public async Task<(bool Success, string? Error, VehicleServiceRecordDto? Record)> RecordServiceAsync(Guid vehicleId, RecordVehicleServiceRequest request, CancellationToken ct = default)
    {
        try
        {
            request.Id = vehicleId;
            var response = await _httpClient.PostAsJsonAsync(BuildUrl($"api/vehicles/{vehicleId}/services"), request, ct);
            if (response.IsSuccessStatusCode)
            {
                var record = await response.Content.ReadFromJsonAsync<VehicleServiceRecordDto>(cancellationToken: ct);
                return (true, null, record);
            }

            var err = await TryExtractErrorMessageAsync(response, ct);
            return (false, err, null);
        }
        catch (Exception ex)
        {
            return (false, ex.Message, null);
        }
    }

    public async Task<bool> DeleteServiceRecordAsync(Guid vehicleId, Guid serviceId, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync(BuildUrl($"api/vehicles/{vehicleId}/services/{serviceId}"), ct);
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
            var text = await response.Content.ReadAsStringAsync(ct);
            if (string.IsNullOrWhiteSpace(text)) return $"Request failed with status {response.StatusCode}.";

            using var doc = JsonDocument.Parse(text);
            if (doc.RootElement.TryGetProperty("error", out var errorProp))
            {
                return errorProp.GetString() ?? text;
            }
            if (doc.RootElement.TryGetProperty("errors", out var errorsProp))
            {
                return errorsProp.ToString();
            }

            return text;
        }
        catch
        {
            return $"Request failed with status {response.StatusCode}.";
        }
    }
}
