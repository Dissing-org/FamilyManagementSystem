using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using FamilyManagement.Api.Endpoints.Freezers;
using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Domain.Enums;
using Xunit;

namespace FamilyManagement.IntegrationTests;

public class FreezersApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public FreezersApiTests(WebApplicationFactory<Program> factory)
    {
        var testDbName = $"test_freezers_{Guid.NewGuid():N}.db";
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:ReceiptDatabase", $"Data Source={testDbName}");
        });
    }

    [Fact]
    public async Task Freezer_FullLifecycle_ApiIntegration()
    {
        var client = _factory.CreateClient();

        // 1. Initial Dashboard Check
        var dashInitResponse = await client.GetAsync("/api/freezers/dashboard");
        dashInitResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var initialDashboard = await dashInitResponse.Content.ReadFromJsonAsync<FreezerDashboardDto>();
        initialDashboard.Should().NotBeNull();

        // 2. Create Freezer
        var createFreezerReq = new CreateFreezerRequest
        {
            Name = "Kummefryser Kælder",
            Location = "Kælder",
            DisplayOrder = 2
        };
        var createFreezerRes = await client.PostAsJsonAsync("/api/freezers", createFreezerReq);
        createFreezerRes.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdFreezer = await createFreezerRes.Content.ReadFromJsonAsync<FreezerDto>();
        createdFreezer.Should().NotBeNull();
        createdFreezer!.Name.Should().Be("Kummefryser Kælder");
        createdFreezer.Location.Should().Be("Kælder");
        var freezerId = createdFreezer.Id;

        // 3. Get Freezer By ID
        var getFreezerRes = await client.GetAsync($"/api/freezers/{freezerId}");
        getFreezerRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var fetchedFreezer = await getFreezerRes.Content.ReadFromJsonAsync<FreezerDto>();
        fetchedFreezer.Should().NotBeNull();
        fetchedFreezer!.Id.Should().Be(freezerId);

        // 4. Update Freezer
        var updateFreezerReq = new UpdateFreezerRequest
        {
            Id = freezerId,
            Name = "Stor Kummefryser Kælder",
            Location = "Kælderrum 3",
            DisplayOrder = 1
        };
        var updateFreezerRes = await client.PutAsJsonAsync($"/api/freezers/{freezerId}", updateFreezerReq);
        updateFreezerRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedFreezer = await updateFreezerRes.Content.ReadFromJsonAsync<FreezerDto>();
        updatedFreezer!.Name.Should().Be("Stor Kummefryser Kælder");

        // 5. Add Compartment
        var addCompReq = new CreateCompartmentRequest
        {
            FreezerId = freezerId,
            Name = "Venstre kurv",
            DisplayOrder = 1
        };
        var addCompRes = await client.PostAsJsonAsync($"/api/freezers/{freezerId}/compartments", addCompReq);
        addCompRes.StatusCode.Should().Be(HttpStatusCode.Created);
        var compDto = await addCompRes.Content.ReadFromJsonAsync<FreezerCompartmentDto>();
        compDto.Should().NotBeNull();
        compDto!.Name.Should().Be("Venstre kurv");
        var compId = compDto.Id;

        // 6. Create Freezer Items
        var createItemReq = new CreateFreezerItemRequest
        {
            CompartmentId = compId,
            Name = "Hakket Oksekød 500g",
            Category = FreezerCategory.MeatAndPoultry,
            Quantity = 4,
            Unit = FreezerUnit.Packages,
            FreezeDate = DateTime.UtcNow.AddDays(-10),
            ExpiryDate = DateTime.UtcNow.AddMonths(3),
            MinimumQuantity = 2,
            Notes = "Økologisk fra slagteren"
        };
        var createItemRes = await client.PostAsJsonAsync("/api/freezer-items", createItemReq);
        createItemRes.StatusCode.Should().Be(HttpStatusCode.Created);
        var itemDto = await createItemRes.Content.ReadFromJsonAsync<FreezerItemDto>();
        itemDto.Should().NotBeNull();
        itemDto!.Name.Should().Be("Hakket Oksekød 500g");
        itemDto.Quantity.Should().Be(4);
        itemDto.IsLowStock.Should().BeFalse();
        var itemId = itemDto.Id;

        // 7. List Items
        var listItemsRes = await client.GetAsync("/api/freezer-items");
        listItemsRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var itemsList = await listItemsRes.Content.ReadFromJsonAsync<List<FreezerItemDto>>();
        itemsList.Should().NotBeNull();
        itemsList!.Should().Contain(i => i.Id == itemId);

        // 8. Consume Freezer Item
        var consumeReq = new ConsumeFreezerItemRequest
        {
            Id = itemId,
            QuantityToConsume = 3,
            Reason = ConsumptionReason.Consumed,
            Notes = "Tacos aften"
        };
        var consumeRes = await client.PostAsJsonAsync($"/api/freezer-items/{itemId}/consume", consumeReq);
        consumeRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var consumedItem = await consumeRes.Content.ReadFromJsonAsync<FreezerItemDto>();
        consumedItem.Should().NotBeNull();
        consumedItem!.Quantity.Should().Be(1);
        consumedItem.IsLowStock.Should().BeTrue(); // MinimumQuantity is 2, quantity is 1!

        // 9. Update Freezer Item
        var updateItemReq = new UpdateFreezerItemRequest
        {
            Id = itemId,
            CompartmentId = compId,
            Name = "Hakket Oksekød 500g (Dansk)",
            Category = FreezerCategory.MeatAndPoultry,
            Quantity = 3,
            Unit = FreezerUnit.Packages,
            FreezeDate = itemDto.FreezeDate,
            ExpiryDate = itemDto.ExpiryDate,
            MinimumQuantity = 2,
            Notes = "Købt ekstra"
        };
        var updateItemRes = await client.PutAsJsonAsync($"/api/freezer-items/{itemId}", updateItemReq);
        updateItemRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var updatedItem = await updateItemRes.Content.ReadFromJsonAsync<FreezerItemDto>();
        updatedItem!.Quantity.Should().Be(3);
        updatedItem.IsLowStock.Should().BeFalse();

        // 10. Dashboard Verification
        var dashRes = await client.GetAsync("/api/freezers/dashboard");
        dashRes.StatusCode.Should().Be(HttpStatusCode.OK);
        var dashDto = await dashRes.Content.ReadFromJsonAsync<FreezerDashboardDto>();
        dashDto.Should().NotBeNull();
        dashDto!.Freezers.Should().Contain(f => f.Id == freezerId);
        dashDto.ItemsByCategory.Should().ContainKey("MeatAndPoultry");

        // 11. Delete Item
        var deleteItemRes = await client.DeleteAsync($"/api/freezer-items/{itemId}");
        deleteItemRes.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 12. Delete Compartment
        var deleteCompRes = await client.DeleteAsync($"/api/freezers/{freezerId}/compartments/{compId}");
        deleteCompRes.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 13. Delete Freezer
        var deleteFreezerRes = await client.DeleteAsync($"/api/freezers/{freezerId}");
        deleteFreezerRes.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
