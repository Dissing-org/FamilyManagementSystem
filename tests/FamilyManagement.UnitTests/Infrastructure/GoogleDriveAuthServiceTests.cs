using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using FamilyManagement.Infrastructure.Storage;
using Xunit;

namespace FamilyManagement.UnitTests.Infrastructure;

public class GoogleDriveAuthServiceTests
{
    [Fact]
    public void IsConfigured_WhenClientIdAndSecretProvided_ShouldBeTrue()
    {
        var options = Options.Create(new GoogleDriveOptions
        {
            ClientId = "test-client-id",
            ClientSecret = "test-client-secret"
        });

        var service = new GoogleDriveAuthService(options, NullLogger<GoogleDriveAuthService>.Instance);
        service.IsConfigured.Should().BeTrue();
    }

    [Fact]
    public void IsConfigured_WhenCredentialsEmpty_ShouldBeFalse()
    {
        var options = Options.Create(new GoogleDriveOptions
        {
            ClientId = "",
            ClientSecret = ""
        });

        var service = new GoogleDriveAuthService(options, NullLogger<GoogleDriveAuthService>.Instance);
        service.IsConfigured.Should().BeFalse();
    }

    [Fact]
    public async Task IsAuthorizedAsync_WhenUnconfigured_ShouldReturnFalse()
    {
        var options = Options.Create(new GoogleDriveOptions());
        var service = new GoogleDriveAuthService(options, NullLogger<GoogleDriveAuthService>.Instance);

        var isAuthorized = await service.IsAuthorizedAsync();
        isAuthorized.Should().BeFalse();
    }

    [Fact]
    public void GetAuthorizationUrl_WhenConfigured_ShouldContainGoogleAuthEndpointAndRedirectUri()
    {
        var options = Options.Create(new GoogleDriveOptions
        {
            ClientId = "test-client-id.apps.googleusercontent.com",
            ClientSecret = "test-secret"
        });

        var service = new GoogleDriveAuthService(options, NullLogger<GoogleDriveAuthService>.Instance);
        var url = service.GetAuthorizationUrl("http://localhost:5270/api/auth/google/callback");

        url.Should().Contain("accounts.google.com");
        url.Should().Contain("client_id=test-client-id.apps.googleusercontent.com");
        url.Should().Contain("redirect_uri=http%3A%2F%2Flocalhost%3A5270%2Fapi%2Fauth%2Fgoogle%2Fcallback");
    }
}
