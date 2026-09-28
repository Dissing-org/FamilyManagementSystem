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
