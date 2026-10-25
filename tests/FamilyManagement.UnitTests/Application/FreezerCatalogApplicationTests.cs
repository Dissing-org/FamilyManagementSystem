using FamilyManagement.Application.DTOs.Freezers;
using FamilyManagement.Application.UseCases.Freezers.Commands;
using FamilyManagement.Application.UseCases.Freezers.Queries;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FamilyManagement.UnitTests.Application;

public class FreezerCatalogApplicationTests
{
    public class FakeFreezerCatalogRepository : IFreezerCatalogRepository
    {
        public readonly List<FreezerCatalogItem> Items = new();

        public Task<IReadOnlyList<FreezerCatalogItem>> GetAllAsync(CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<FreezerCatalogItem>>(Items.ToList());

        public Task<FreezerCatalogItem?> GetByIdAsync(FreezerCatalogItemId id, CancellationToken ct = default) =>
            Task.FromResult(Items.FirstOrDefault(i => i.Id == id));

        public Task<FreezerCatalogItem?> GetByNameAsync(string name, CancellationToken ct = default) =>
            Task.FromResult(Items.FirstOrDefault(i => string.Equals(i.Name, name.Trim(), StringComparison.OrdinalIgnoreCase)));

        public Task<FreezerCatalogItem> AddAsync(FreezerCatalogItem item, CancellationToken ct = default)
        {
            Items.Add(item);
            return Task.FromResult(item);
        }

        public Task UpdateAsync(FreezerCatalogItem item, CancellationToken ct = default) => Task.CompletedTask;

        public Task DeleteAsync(FreezerCatalogItemId id, CancellationToken ct = default)
        {
            Items.RemoveAll(i => i.Id == id);
            return Task.CompletedTask;
        }
    }

    public class FakeFreezerItemRepo : IFreezerItemRepository
    {
        public readonly List<FreezerItem> Items = new();

        public Task<IReadOnlyList<FreezerItem>> GetAllAsync(CancellationToken ct = default) => Task.FromResult<IReadOnlyList<FreezerItem>>(Items);
        public Task<IReadOnlyList<FreezerItem>> GetByCompartmentIdAsync(FreezerCompartmentId compartmentId, CancellationToken ct = default) => Task.FromResult<IReadOnlyList<FreezerItem>>(Items);
        public Task<FreezerItem?> GetByIdAsync(FreezerItemId id, CancellationToken ct = default) => Task.FromResult(Items.FirstOrDefault(i => i.Id == id));
        public Task AddAsync(FreezerItem item, CancellationToken ct = default) { Items.Add(item); return Task.CompletedTask; }
        public Task UpdateAsync(FreezerItem item, CancellationToken ct = default) => Task.CompletedTask;
        public Task DeleteAsync(FreezerItemId id, CancellationToken ct = default) { Items.RemoveAll(i => i.Id == id); return Task.CompletedTask; }
        public Task AddTransactionAsync(FreezerTransaction transaction, CancellationToken ct = default) => Task.CompletedTask;
        public Task<IReadOnlyList<FreezerTransaction>> GetTransactionsByItemIdAsync(FreezerItemId itemId, CancellationToken ct = default) => Task.FromResult<IReadOnlyList<FreezerTransaction>>(new List<FreezerTransaction>());
    }

    [Fact]
    public async Task GetFreezerCatalogQuery_ReturnsAlphabeticallyOrderedItems()
    {
        var repo = new FakeFreezerCatalogRepository();
        repo.Items.Add(FreezerCatalogItem.Create("Suppe", FreezerCategory.PreparedMeals, FreezerUnit.Portions));
        repo.Items.Add(FreezerCatalogItem.Create("Agurk", FreezerCategory.VegetablesAndFruits, FreezerUnit.Pieces));
        repo.Items.Add(FreezerCatalogItem.Create("Bøf", FreezerCategory.MeatAndPoultry, FreezerUnit.Packages));

        var handler = new GetFreezerCatalogQueryHandler(repo);
        var result = await handler.HandleAsync(new GetFreezerCatalogQuery());

        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Agurk");
        result[1].Name.Should().Be("Bøf");
        result[2].Name.Should().Be("Suppe");
    }

    [Fact]
    public async Task UpsertFreezerCatalogItemCommand_WhenItemDoesNotExist_CreatesItem()
    {
        var repo = new FakeFreezerCatalogRepository();
        var handler = new UpsertFreezerCatalogItemCommandHandler(repo);

        var result = await handler.HandleAsync(new UpsertFreezerCatalogItemCommand(
            "Torskefilet",
            FreezerCategory.Seafood,
            FreezerUnit.Packages,
            1));

        result.Name.Should().Be("Torskefilet");
        result.DefaultCategory.Should().Be(FreezerCategory.Seafood);
        repo.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task UpsertFreezerCatalogItemCommand_WhenItemExists_UpdatesDefaults()
    {
        var repo = new FakeFreezerCatalogRepository();
        var existing = FreezerCatalogItem.Create("Kylling", FreezerCategory.MeatAndPoultry, FreezerUnit.Pieces);
        repo.Items.Add(existing);

        var handler = new UpsertFreezerCatalogItemCommandHandler(repo);
        var result = await handler.HandleAsync(new UpsertFreezerCatalogItemCommand(
            "kylling",
            FreezerCategory.MeatAndPoultry,
            FreezerUnit.Kilograms,
            2));

        result.DefaultUnit.Should().Be(FreezerUnit.Kilograms);
        result.DefaultMinimumQuantity.Should().Be(2);
        repo.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task DeleteFreezerCatalogItemCommand_RemovesItemFromRepository()
    {
        var repo = new FakeFreezerCatalogRepository();
        var existing = FreezerCatalogItem.Create("Fejltastet Vare", FreezerCategory.Other, FreezerUnit.Pieces);
        repo.Items.Add(existing);

        var handler = new DeleteFreezerCatalogItemCommandHandler(repo);
        var result = await handler.HandleAsync(new DeleteFreezerCatalogItemCommand(existing.Id.Value));

        result.Should().BeTrue();
        repo.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateFreezerItemCommand_AutoUpsertsCatalogItem()
    {
        var itemRepo = new FakeFreezerItemRepo();
        var catalogRepo = new FakeFreezerCatalogRepository();
        var handler = new CreateFreezerItemCommandHandler(itemRepo, catalogRepo);

        var compartmentId = Guid.NewGuid();
        var result = await handler.HandleAsync(new CreateFreezerItemCommand(
            compartmentId,
            "Hakket Oksekød",
            FreezerCategory.MeatAndPoultry,
            3,
            FreezerUnit.Packages,
            DateTime.UtcNow,
            null,
            1,
            "500g pakker"));

        result.Name.Should().Be("Hakket Oksekød");
        catalogRepo.Items.Should().ContainSingle();
        var catalogItem = catalogRepo.Items.Single();
        catalogItem.Name.Should().Be("Hakket Oksekød");
        catalogItem.DefaultCategory.Should().Be(FreezerCategory.MeatAndPoultry);
        catalogItem.DefaultUnit.Should().Be(FreezerUnit.Packages);
    }
}
