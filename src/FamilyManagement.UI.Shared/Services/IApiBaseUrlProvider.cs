namespace FamilyManagement.UI.Shared.Services;

public interface IApiBaseUrlProvider
{
    string GetBaseUrl();
    void SetBaseUrl(string url);
}

public class DefaultApiBaseUrlProvider : IApiBaseUrlProvider
{
    private string _baseUrl;

    public DefaultApiBaseUrlProvider(string defaultUrl = "")
    {
        _baseUrl = defaultUrl;
    }

    public string GetBaseUrl() => _baseUrl;

    public void SetBaseUrl(string url)
    {
        _baseUrl = url.TrimEnd('/');
    }
}

public static class ApiBaseUrlProviderExtensions
{
    public static string ResolveUrl(this IApiBaseUrlProvider provider, HttpClient httpClient, string relativePath)
    {
        var baseUri = provider.GetBaseUrl();
        var trimmed = relativePath.TrimStart('/');
        if (string.IsNullOrWhiteSpace(baseUri))
        {
            return trimmed;
        }

        if (httpClient.BaseAddress != null &&
            Uri.TryCreate(baseUri, UriKind.Absolute, out var parsedBase) &&
            string.Equals(parsedBase.Host, httpClient.BaseAddress.Host, StringComparison.OrdinalIgnoreCase))
        {
            return trimmed;
        }

        return $"{baseUri.TrimEnd('/')}/{trimmed}";
    }
}
