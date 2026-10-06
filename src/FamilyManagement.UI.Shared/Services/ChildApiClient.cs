using System.Net.Http.Json;
using FamilyManagement.UI.Shared.Models.Children;

namespace FamilyManagement.UI.Shared.Services;

public class ChildApiClient
{
    private readonly HttpClient _httpClient;

    public ChildApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ChildProfileModel>> ListChildrenAsync(CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<ChildProfileModel>>("/api/children", ct);
            return response ?? new List<ChildProfileModel>();
        }
        catch
        {
            return new List<ChildProfileModel>();
        }
    }

    public async Task<ChildProfileModel?> CreateChildAsync(
        string firstName,
        string? lastName,
        DateTime dateOfBirth,
        string gender,
        string? avatarUrl,
        string? clothesSize,
        string? shoeSize,
        string? hatSize,
        string? diaperSize,
        CancellationToken ct = default)
    {
        var payload = new
        {
            firstName,
            lastName,
            dateOfBirth,
            gender,
            avatarUrl,
            clothesSize,
            shoeSize,
            hatSize,
            diaperSize
        };

        var response = await _httpClient.PostAsJsonAsync("/api/children", payload, ct);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<ChildProfileModel>(cancellationToken: ct);
    }

    public async Task<ChildDashboardModel?> GetDashboardAsync(Guid childId, CancellationToken ct = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ChildDashboardModel>($"/api/children/{childId}/dashboard", ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> UpdateSizesAsync(
        Guid childId,
        string? clothesSize,
        string? shoeSize,
        string? hatSize,
        string? diaperSize,
        CancellationToken ct = default)
    {
        var payload = new { id = childId, clothesSize, shoeSize, hatSize, diaperSize };
        var response = await _httpClient.PutAsJsonAsync($"/api/children/{childId}/sizes", payload, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<GrowthMeasurementModel>> GetGrowthHistoryAsync(Guid childId, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<GrowthMeasurementModel>>($"/api/children/{childId}/growth", ct);
            return response ?? new List<GrowthMeasurementModel>();
        }
        catch
        {
            return new List<GrowthMeasurementModel>();
        }
    }

    public async Task<GrowthMeasurementModel?> RecordGrowthAsync(
        Guid childId,
        DateTime recordedDate,
        decimal? heightCm,
        decimal? weightKg,
        decimal? headCircumferenceCm,
        string? notes,
        CancellationToken ct = default)
    {
        var payload = new
        {
            childId,
            recordedDate,
            heightCm,
            weightKg,
            headCircumferenceCm,
            notes
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/children/{childId}/growth", payload, ct);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<GrowthMeasurementModel>(cancellationToken: ct);
    }

    public async Task<bool> DeleteGrowthAsync(Guid measurementId, CancellationToken ct = default)
    {
        var response = await _httpClient.DeleteAsync($"/api/children/growth/{measurementId}", ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<List<ChildMilestoneModel>> GetMilestonesAsync(
        Guid childId,
        string? status = null,
        string? category = null,
        CancellationToken ct = default)
    {
        try
        {
            var url = $"/api/children/{childId}/milestones";
            var query = new List<string>();
            if (!string.IsNullOrEmpty(status)) query.Add($"status={status}");
            if (!string.IsNullOrEmpty(category)) query.Add($"category={category}");
            if (query.Count > 0) url += "?" + string.Join("&", query);

            var response = await _httpClient.GetFromJsonAsync<List<ChildMilestoneModel>>(url, ct);
            return response ?? new List<ChildMilestoneModel>();
        }
        catch
        {
            return new List<ChildMilestoneModel>();
        }
    }

    public async Task<ChildMilestoneModel?> CreateMilestoneAsync(
        Guid childId,
        string title,
        string category,
        bool isAchieved,
        int? expectedAgeMonths,
        int? expectedWindowMaxMonths,
        DateTime? achievedDate,
        string? description,
        string? notes,
        string? photoUrl,
        CancellationToken ct = default)
    {
        var payload = new
        {
            childId,
            title,
            category,
            isAchieved,
            expectedAgeMonths,
            expectedWindowMaxMonths,
            achievedDate,
            description,
            notes,
            photoUrl
        };

        var response = await _httpClient.PostAsJsonAsync($"/api/children/{childId}/milestones", payload, ct);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<ChildMilestoneModel>(cancellationToken: ct);
    }

    public async Task<bool> MarkMilestoneAchievedAsync(
        Guid milestoneId,
        DateTime achievedDate,
        string? notes,
        string? photoUrl = null,
        CancellationToken ct = default)
    {
        var payload = new { id = milestoneId, achievedDate, notes, photoUrl };
        var response = await _httpClient.PutAsJsonAsync($"/api/children/milestones/{milestoneId}/achieve", payload, ct);
        return response.IsSuccessStatusCode;
    }

    public async Task<int> SeedStandardMilestonesAsync(Guid childId, CancellationToken ct = default)
    {
        var response = await _httpClient.PostAsJsonAsync($"/api/children/{childId}/milestones/seed", new { childId }, ct);
        if (!response.IsSuccessStatusCode) return 0;
        var res = await response.Content.ReadFromJsonAsync<SeedResponse>(cancellationToken: ct);
        return res?.AddedCount ?? 0;
    }

    private class SeedResponse
    {
        public int AddedCount { get; set; }
    }
}
