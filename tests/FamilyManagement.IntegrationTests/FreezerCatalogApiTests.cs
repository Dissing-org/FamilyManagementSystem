using System.Net;
using System.Net.Http.Json;
using FamilyManagement.Api.Endpoints.Freezers;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace FamilyManagement.IntegrationTests;

public class FreezerCatalogApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public FreezerCatalogApiTests(WebApplicationFactory<Program> factory)
    {
        var testDbName = $"test_freezer_catalog_{Guid.NewGuid():N}.db";
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:ReceiptDatabase", $"Data Source={testDbName}");
        });
    }

    [Fact]
    public async Task FreezerCatalog_FullLifecycle_ApiIntegration()
    {
        var client = _factory.CreateClient();

        // 1. Initial Catalog Check (empty or seeded)
        var initRes = await client.GetAsync("/api/freezers/catalog");
        initRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var initialList = await initRes.Content.ReadFromJsonAsync<List<FreezerCatalogItemDto>>();
        initialList.Should().NotBeNull();

        // 2. Upsert new item to catalog
        var upsertReq = new UpsertFreezerCatalogItemRequest
        {
            Name = "Hakket Oksekød",
            Category = FreezerCategory.MeatAndPoultry,
            Unit = FreezerUnit.Packages,
            MinimumQuantity = 2
        };

        var upsertRes = await client.PostAsJsonAsync("/api/freezers/catalog", upsertReq);
        upsertRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var upsertedItem = await upsertRes.Content.ReadFromJsonAsync<FreezerCatalogItemDto>();
        upsertedItem.Should().NotBeNull();
        upsertedItem!.Name.Should().Be("Hakket Oksekød");
        upsertedItem.DefaultCategory.Should().Be(FreezerCategory.MeatAndPoultry);
        upsertedItem.DefaultUnit.Should().Be(FreezerUnit.Packages);

        // 3. Upsert update existing item
        var updateReq = new UpsertFreezerCatalogItemRequest
        {
            Name = "hakket oksekød",
            Category = FreezerCategory.MeatAndPoultry,
            Unit = FreezerUnit.Kilograms,
            MinimumQuantity = 3
        };

        var updateRes = await client.PostAsJsonAsync("/api/freezers/catalog", updateReq);
        updateRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedItem = await updateRes.Content.ReadFromJsonAsync<FreezerCatalogItemDto>();
        updatedItem!.DefaultUnit.Should().Be(FreezerUnit.Kilograms);
        updatedItem.DefaultMinimumQuantity.Should().Be(3);

        // 4. Verify item exists in list
        var listRes = await client.GetAsync("/api/freezers/catalog");
        var list = await listRes.Content.ReadFromJsonAsync<List<FreezerCatalogItemDto>>();
        list.Should().Contain(i => i.Name == "Hakket Oksekød" && i.DefaultUnit == FreezerUnit.Kilograms);

        // 5. Delete item from catalog
        var deleteRes = await client.DeleteAsync($"/api/freezers/catalog/{upsertedItem.Id}");
        deleteRes.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 6. Verify item is deleted from suggestions
        var finalListRes = await client.GetAsync("/api/freezers/catalog");
        var finalList = await finalListRes.Content.ReadFromJsonAsync<List<FreezerCatalogItemDto>>();
        finalList.Should().NotContain(i => i.Id == upsertedItem.Id);
    }
}
