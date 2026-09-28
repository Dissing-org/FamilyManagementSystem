using System.Globalization;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FamilyManagement.UI.Shared.Models;

namespace FamilyManagement.UI.Shared.Services;

public interface IReceiptApiClient
{
    Task<List<ReceiptViewModel>> GetReceiptsAsync(string? merchant = null, CancellationToken ct = default);
    Task<ReceiptViewModel?> GetReceiptByIdAsync(Guid id, CancellationToken ct = default);
    Task<ReceiptViewModel> UploadReceiptAsync(UploadReceiptModel model, CancellationToken ct = default);
    Task<bool> DeleteReceiptAsync(Guid id, CancellationToken ct = default);
}

public class ReceiptApiClient : IReceiptApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IApiBaseUrlProvider _baseUrlProvider;

    public ReceiptApiClient(HttpClient httpClient, IApiBaseUrlProvider baseUrlProvider)
    {
        _httpClient = httpClient;
        _baseUrlProvider = baseUrlProvider;
    }

    private string BuildUrl(string relativePath)
    {
        var baseUri = _baseUrlProvider.GetBaseUrl();
        if (string.IsNullOrWhiteSpace(baseUri))
        {
            return relativePath;
        }

        return $"{baseUri.TrimEnd('/')}/{relativePath.TrimStart('/')}";
    }

    public async Task<List<ReceiptViewModel>> GetReceiptsAsync(string? merchant = null, CancellationToken ct = default)
    {
        var url = string.IsNullOrWhiteSpace(merchant)
            ? BuildUrl("api/receipts")
            : BuildUrl($"api/receipts?merchant={Uri.EscapeDataString(merchant)}");

        var response = await _httpClient.GetFromJsonAsync<List<ReceiptViewModel>>(url, ct);
        return response ?? new List<ReceiptViewModel>();
    }

    public async Task<ReceiptViewModel?> GetReceiptByIdAsync(Guid id, CancellationToken ct = default)
    {
        var url = BuildUrl($"api/receipts/{id}");
        var response = await _httpClient.GetAsync(url, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ReceiptViewModel>(cancellationToken: ct);
    }

    public async Task<ReceiptViewModel> UploadReceiptAsync(UploadReceiptModel model, CancellationToken ct = default)
    {
        using var form = new MultipartFormDataContent();
        form.Add(new StringContent(model.Merchant), "Merchant");
        form.Add(new StringContent(model.PurchaseDate.ToString("o")), "PurchaseDate");
        form.Add(new StringContent(model.Amount.ToString("F2", CultureInfo.InvariantCulture)), "Amount");
        form.Add(new StringContent(model.Currency), "Currency");
        form.Add(new StringContent(model.Category), "Category");

        if (!string.IsNullOrWhiteSpace(model.Notes))
        {
            form.Add(new StringContent(model.Notes), "Notes");
        }

        if (model.FileBytes is not null && model.FileBytes.Length > 0)
        {
            var fileContent = new ByteArrayContent(model.FileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.ContentType ?? "application/octet-stream");
            form.Add(fileContent, "File", model.FileName ?? "receipt.pdf");
        }
        else
        {
            throw new ArgumentException("A receipt file must be selected.", nameof(model));
        }

        var url = BuildUrl("api/receipts");
        var response = await _httpClient.PostAsync(url, form, ct);
        
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync(ct);
            throw new InvalidOperationException($"Upload failed ({response.StatusCode}): {error}");
        }

        var created = await response.Content.ReadFromJsonAsync<ReceiptViewModel>(cancellationToken: ct);
        return created ?? throw new InvalidOperationException("Failed to deserialize created receipt response.");
    }

    public async Task<bool> DeleteReceiptAsync(Guid id, CancellationToken ct = default)
    {
        var url = BuildUrl($"api/receipts/{id}");
        var response = await _httpClient.DeleteAsync(url, ct);
        return response.IsSuccessStatusCode;
    }
}
