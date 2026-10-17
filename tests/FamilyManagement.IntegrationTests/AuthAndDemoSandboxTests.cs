using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using FamilyManagement.Api.Auth;
using FamilyManagement.Api.Endpoints.Auth;
using FamilyManagement.Application.DTOs;
using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Application.Vehicles;
using Xunit;

namespace FamilyManagement.IntegrationTests;

public class AuthAndDemoSandboxTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthAndDemoSandboxTests(WebApplicationFactory<Program> factory)
    {
        var ownerDbName = $"test_owner_{Guid.NewGuid():N}.db";
        var demoDbName = $"test_demo_{Guid.NewGuid():N}.db";
        var demoStorageDir = $"test_demo_storage_{Guid.NewGuid():N}";

        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:ReceiptDatabase", $"Data Source={ownerDbName}");
            builder.UseSetting("ConnectionStrings:DemoDatabase", $"Data Source={demoDbName}");
            builder.UseSetting("GoogleDrive:DemoLocalStorageDirectory", demoStorageDir);
            builder.UseSetting("Auth:AllowDevelopmentOwnerFallback", "false");
            builder.UseSetting("Auth:OwnerEmail", "mathi@familyhub.dk");
            builder.UseSetting("Auth:OwnerDisplayName", "Mathi");
            builder.UseSetting("Auth:DemoEmail", "demo@familyhub.dk");
            builder.UseSetting("Auth:DemoPassword", "FamilyHubDemo2026!");
            builder.UseSetting("Auth:DemoDisplayName", "Guest");
        });
    }

    [Theory]
    [InlineData("/api/receipts")]
    [InlineData("/api/insurances")]
    [InlineData("/api/vehicles")]
    [InlineData("/api/children")]
    public async Task UnauthenticatedRequests_WhenFallbackDisabled_ShouldReturn401Unauthorized(string path)
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync(path);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("mathi@familyhub.dk")]
    [InlineData("owner@dissing.dev")]
    public async Task CloudflareHeader_WithNonDemoEmail_ShouldAuthenticateAsOwner_AndForbidDemoReset(string email)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add(HybridAuthenticationHandler.CfAccessEmailHeader, email);

        var sessionResponse = await client.GetAsync("/api/auth/session");
        sessionResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var session = await sessionResponse.Content.ReadFromJsonAsync<AuthSessionResponse>();
        session.Should().NotBeNull();
        session!.IsAuthenticated.Should().BeTrue();
        session.Role.Should().Be("Owner");
        session.Email.Should().Be(email);
        session.DisplayName.Should().Be("Mathi");

        var receiptsResponse = await client.GetAsync("/api/receipts");
        receiptsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var ownerReceipts = await receiptsResponse.Content.ReadFromJsonAsync<List<ReceiptResponseDto>>();
        ownerReceipts.Should().NotBeNull();
        ownerReceipts.Should().BeEmpty();

        var resetResponse = await client.PostAsync("/api/demo/reset", null);
        resetResponse.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CloudflareHeader_WithDemoEmail_ShouldRequireDemoLogin_AndThenAuthenticateAsDemo()
    {
        var demoClient = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true
        });
        demoClient.DefaultRequestHeaders.Add(HybridAuthenticationHandler.CfAccessEmailHeader, "demo@familyhub.dk");

        var initialSession = await demoClient.GetFromJsonAsync<AuthSessionResponse>("/api/auth/session");
        initialSession.Should().NotBeNull();
        initialSession!.IsAuthenticated.Should().BeFalse();

        var unauthReceiptsResponse = await demoClient.GetAsync("/api/receipts");
        unauthReceiptsResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var loginResponse = await demoClient.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = "demo@familyhub.dk",
            Password = "FamilyHubDemo2026!"
        });
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var activeSession = await demoClient.GetFromJsonAsync<AuthSessionResponse>("/api/auth/session");
        activeSession.Should().NotBeNull();
        activeSession!.IsAuthenticated.Should().BeTrue();
        activeSession.Role.Should().Be("Demo");

        var demoReceiptsResponse = await demoClient.GetAsync("/api/receipts");
        demoReceiptsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ShouldReturn401Unauthorized()
    {
        var client = _factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = "demo@familyhub.dk",
            Password = "WrongPassword123!"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DemoLogin_ShouldRouteToSeededDanishShowcase_WhileOwnerDatabaseRemainsUntouched()
    {
        var demoClient = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true
        });

        var loginResponse = await demoClient.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = "demo@familyhub.dk",
            Password = "FamilyHubDemo2026!"
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginSession = await loginResponse.Content.ReadFromJsonAsync<AuthSessionResponse>();
        loginSession.Should().NotBeNull();
        loginSession!.IsAuthenticated.Should().BeTrue();
        loginSession.Role.Should().Be("Demo");
        loginSession.DisplayName.Should().Be("Guest");

        var sessionCheck = await demoClient.GetFromJsonAsync<AuthSessionResponse>("/api/auth/session");
        sessionCheck.Should().NotBeNull();
        sessionCheck!.IsAuthenticated.Should().BeTrue();
        sessionCheck.Role.Should().Be("Demo");
        sessionCheck.DisplayName.Should().Be("Guest");

        var demoReceipts = await demoClient.GetFromJsonAsync<List<ReceiptResponseDto>>("/api/receipts");
        demoReceipts.Should().NotBeNull();
        demoReceipts!.Count.Should().Be(8);
        demoReceipts.Select(r => r.Merchant).Should().Contain(["Føtex Østerbro", "IKEA Gentofte", "LEGO Store København"]);

        var firstReceiptFileResponse = await demoClient.GetAsync(demoReceipts[0].WebViewLink);
        firstReceiptFileResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        firstReceiptFileResponse.Content.Headers.ContentType?.MediaType.Should().Be("application/pdf");

        var demoVehicles = await demoClient.GetFromJsonAsync<List<VehicleDto>>("/api/vehicles");
        demoVehicles.Should().NotBeNull();
        demoVehicles!.Count.Should().Be(2);
        demoVehicles.Select(v => v.Make).Should().Contain(["Volvo", "Tesla"]);

        var demoChildren = await demoClient.GetFromJsonAsync<List<ChildProfileDto>>("/api/children");
        demoChildren.Should().NotBeNull();
        demoChildren!.Count.Should().Be(2);
        demoChildren.Select(c => c.FirstName).Should().Contain(["Astrid", "Felix"]);

        var demoInsurances = await demoClient.GetFromJsonAsync<List<InsurancePolicyResponseDto>>("/api/insurances");
        demoInsurances.Should().NotBeNull();
        demoInsurances!.Count.Should().Be(4);
        demoInsurances.Select(i => i.Insurer).Should().Contain(["Tryg Forsikring", "Topdanmark"]);

        var ownerClient = _factory.CreateClient();
        ownerClient.DefaultRequestHeaders.Add(HybridAuthenticationHandler.CfAccessEmailHeader, "mathi@familyhub.dk");
        var ownerReceipts = await ownerClient.GetFromJsonAsync<List<ReceiptResponseDto>>("/api/receipts");
        ownerReceipts.Should().NotBeNull();
        ownerReceipts.Should().BeEmpty();
    }

    [Fact]
    public async Task DemoSandbox_ModifyAndReset_ShouldRestoreExactSeededShowcaseDataset()
    {
        var demoClient = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            HandleCookies = true
        });

        var loginResponse = await demoClient.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = "demo@familyhub.dk",
            Password = "FamilyHubDemo2026!"
        });
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var initialReceipts = await demoClient.GetFromJsonAsync<List<ReceiptResponseDto>>("/api/receipts");
        initialReceipts.Should().NotBeNull();
        initialReceipts!.Count.Should().Be(8);

        var deleteResponse = await demoClient.DeleteAsync($"/api/receipts/{initialReceipts[0].Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var afterDeleteReceipts = await demoClient.GetFromJsonAsync<List<ReceiptResponseDto>>("/api/receipts");
        afterDeleteReceipts!.Count.Should().Be(7);

        var resetResponse = await demoClient.PostAsync("/api/demo/reset", null);
        resetResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var restoredReceipts = await demoClient.GetFromJsonAsync<List<ReceiptResponseDto>>("/api/receipts");
        restoredReceipts.Should().NotBeNull();
        restoredReceipts!.Count.Should().Be(8);

        var restoredVehicles = await demoClient.GetFromJsonAsync<List<VehicleDto>>("/api/vehicles");
        restoredVehicles!.Count.Should().Be(2);

        var restoredChildren = await demoClient.GetFromJsonAsync<List<ChildProfileDto>>("/api/children");
        restoredChildren!.Count.Should().Be(2);

        var restoredInsurances = await demoClient.GetFromJsonAsync<List<InsurancePolicyResponseDto>>("/api/insurances");
        restoredInsurances!.Count.Should().Be(4);
    }
}
