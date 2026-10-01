using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using FamilyManagement.Api.Endpoints.Insurances;
using FamilyManagement.Application.DTOs.Insurances;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Infrastructure.Persistence;
using Xunit;

namespace FamilyManagement.IntegrationTests;

public class InsurancesApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public InsurancesApiTests(WebApplicationFactory<Program> factory)
    {
        var testDbName = $"test_insurances_{Guid.NewGuid():N}.db";
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:ReceiptDatabase", $"Data Source={testDbName}");
        });
    }

    [Fact]
    public async Task Create_Get_Update_Delete_InsurancePolicy_FullLifecycle()
    {
        var client = _factory.CreateClient();

        // 1. Create Insurance Policy
        var createRequest = new CreateInsurancePolicyRequest
        {
            Insurer = "Topdanmark",
            PolicyNumber = "POL-AUTO-101",
            Category = InsuranceCategory.Auto,
            InsuredParty = "Tesla Model 3",
            PremiumAmount = 600m,
            Currency = "DKK",
            Frequency = PaymentFrequency.Quarterly, // 600/qtr = 200/mo, 2400/yr
            StartDate = DateTime.UtcNow.AddMonths(-3),
            RenewalDate = DateTime.UtcNow.AddDays(45), // Within 90 days
            DeductibleAmount = 3000m,
            Notes = "Comprehensive coverage"
        };

        var postResponse = await client.PostAsJsonAsync("/api/insurances", createRequest);
        postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await postResponse.Content.ReadFromJsonAsync<InsurancePolicyResponseDto>();
        created.Should().NotBeNull();
        created!.Insurer.Should().Be("Topdanmark");
        created.PolicyNumber.Should().Be("POL-AUTO-101");
        created.Category.Should().Be("Auto");
        created.InsuredParty.Should().Be("Tesla Model 3");
        created.PremiumAmount.Should().Be(600m);
        created.MonthlyCost.Should().Be(200m);
        created.AnnualCost.Should().Be(2400m);
        created.Status.Should().Be("Active");

        var policyId = created.Id;

        // 2. Get by ID
        var getByIdResponse = await client.GetAsync($"/api/insurances/{policyId}");
        getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetched = await getByIdResponse.Content.ReadFromJsonAsync<InsurancePolicyResponseDto>();
        fetched!.Id.Should().Be(policyId);
        fetched.InsuredParty.Should().Be("Tesla Model 3");

        // 3. List with Category Filter
        var listResponse = await client.GetAsync("/api/insurances?category=Auto");
        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await listResponse.Content.ReadFromJsonAsync<List<InsurancePolicyResponseDto>>();
        list.Should().NotBeNull();
        list!.Should().Contain(p => p.Id == policyId);

        // 4. Get Overview
        var overviewResponse = await client.GetAsync("/api/insurances/overview?daysAhead=60");
        overviewResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var overview = await overviewResponse.Content.ReadFromJsonAsync<InsuranceOverviewDto>();
        overview.Should().NotBeNull();
        overview!.TotalMonthlyCost.Should().BeGreaterThanOrEqualTo(200m);
        overview.TotalAnnualCost.Should().BeGreaterThanOrEqualTo(2400m);
        overview.CostByCategory.Should().ContainKey("Auto");
        overview.UpcomingRenewals.Should().Contain(r => r.Id == policyId);

        // 5. Update Policy
        var updateRequest = new UpdateInsurancePolicyRequest
        {
            Id = policyId,
            Insurer = "Topdanmark Forsikring",
            PolicyNumber = "POL-AUTO-101-REV",
            Category = InsuranceCategory.Auto,
            InsuredParty = "Tesla Model 3 Performance",
            PremiumAmount = 750m,
            Currency = "DKK",
            Frequency = PaymentFrequency.Quarterly,
            RenewalDate = DateTime.UtcNow.AddDays(60),
            DeductibleAmount = 2500m,
            Notes = "Updated with glass coverage"
        };

        var putResponse = await client.PutAsJsonAsync($"/api/insurances/{policyId}", updateRequest);
        putResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await putResponse.Content.ReadFromJsonAsync<InsurancePolicyResponseDto>();
        updated!.Insurer.Should().Be("Topdanmark Forsikring");
        updated.InsuredParty.Should().Be("Tesla Model 3 Performance");
        updated.PremiumAmount.Should().Be(750m);
        updated.MonthlyCost.Should().Be(250m);

        // 6. Delete Policy
        var deleteResponse = await client.DeleteAsync($"/api/insurances/{policyId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 7. Verify Deleted
        var getAfterDelete = await client.GetAsync($"/api/insurances/{policyId}");
        getAfterDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
