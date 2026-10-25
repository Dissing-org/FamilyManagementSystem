using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FamilyManagement.Api.Endpoints.Freezers;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Xunit;
using UiCatalogDto = FamilyManagement.UI.Shared.Models.FreezerCatalogItemDto;

namespace FamilyManagement.IntegrationTests;

public class FreezerCatalogApiTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly string _testDbName;

    public FreezerCatalogApiTests(WebApplicationFactory<Program> factory)
    {
        CleanupOrphanedTestDatabases();
        _testDbName = $"test_freezer_catalog_{Guid.NewGuid():N}.db";
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:ReceiptDatabase", $"Data Source={_testDbName}");
        });
    }

    [Fact]
    public async Task FreezerCatalog_FullLifecycle_ApiIntegration()
    {
        using var client = _factory.CreateClient();

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

        // 3. Upsert update existing item (including Scandinavian ø/Ø and null MinimumQuantity preservation)
        var updateReq = new UpsertFreezerCatalogItemRequest
        {
            Name = "HAKKET OKSEKØD",
            Category = FreezerCategory.MeatAndPoultry,
            Unit = FreezerUnit.Kilograms,
            MinimumQuantity = null
        };

        var updateRes = await client.PostAsJsonAsync("/api/freezers/catalog", updateReq);
        updateRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedItem = await updateRes.Content.ReadFromJsonAsync<FreezerCatalogItemDto>();
        updatedItem!.DefaultUnit.Should().Be(FreezerUnit.Kilograms);
        updatedItem.DefaultMinimumQuantity.Should().Be(2);

        // 4. Scandinavian case-insensitivity ("Æbleskiver" vs "æbleskiver") & UI DTO string enum deserialization
        var scandiReq1 = new UpsertFreezerCatalogItemRequest
        {
            Name = "Æbleskiver",
            Category = FreezerCategory.Bakery,
            Unit = FreezerUnit.Packages,
            MinimumQuantity = 1
        };
        var scandiRes1 = await client.PostAsJsonAsync("/api/freezers/catalog", scandiReq1);
        scandiRes1.StatusCode.Should().Be(HttpStatusCode.OK);

        var scandiReq2 = new UpsertFreezerCatalogItemRequest
        {
            Name = "æbleskiver",
            Category = FreezerCategory.Bakery,
            Unit = FreezerUnit.Pieces,
            MinimumQuantity = 20
        };
        var scandiRes2 = await client.PostAsJsonAsync("/api/freezers/catalog", scandiReq2);
        scandiRes2.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify raw HTTP JSON deserializes cleanly into Blazor UI's string-based FreezerCatalogItemDto
        var listRes = await client.GetAsync("/api/freezers/catalog");
        var rawJson = await listRes.Content.ReadAsStringAsync();
        var uiList = JsonSerializer.Deserialize<List<UiCatalogDto>>(rawJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        uiList.Should().NotBeNull();
        uiList!.Where(i => i.Name.Equals("Æbleskiver", StringComparison.OrdinalIgnoreCase)).Should().ContainSingle();
        var aebleskiver = uiList.Single(i => i.Name.Equals("Æbleskiver", StringComparison.OrdinalIgnoreCase));
        aebleskiver.DefaultCategory.Should().Be("Bakery");
        aebleskiver.DefaultUnit.Should().Be("Pieces");
        aebleskiver.DefaultMinimumQuantity.Should().Be(20);

        // 5. Delete item from catalog
        var deleteRes = await client.DeleteAsync($"/api/freezers/catalog/{upsertedItem.Id}");
        deleteRes.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 6. Verify item is deleted from suggestions
        var finalListRes = await client.GetAsync("/api/freezers/catalog");
        var finalList = await finalListRes.Content.ReadFromJsonAsync<List<FreezerCatalogItemDto>>();
        finalList.Should().NotContain(i => i.Id == upsertedItem.Id);
    }

    public void Dispose()
    {
        _factory.Dispose();
        SqliteConnection.ClearAllPools();
        DeleteDbFiles(_testDbName);
        CleanupOrphanedTestDatabases();
    }

    private static void CleanupOrphanedTestDatabases()
    {
        try
        {
            foreach (var file in Directory.GetFiles(Directory.GetCurrentDirectory(), "test_freezer_catalog_*.db*"))
            {
                try { File.Delete(file); } catch { /* ignore locked files */ }
            }
        }
        catch
        {
            // Ignore cleanup errors
        }
    }

    private static void DeleteDbFiles(string dbName)
    {
        foreach (var suffix in new[] { "", "-wal", "-shm" })
        {
            var path = dbName + suffix;
            if (File.Exists(path))
            {
                try { File.Delete(path); } catch { /* ignore if still locked */ }
            }
        }
    }
}
