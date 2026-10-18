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

public class FreezerApplicationTests
{
    private class FakeFreezerRepository : IFreezerRepository
    {
        public readonly List<Freezer> Freezers = new();

        public Task<IReadOnlyList<Freezer>> GetAllWithCompartmentsAsync(CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<Freezer>>(Freezers.ToList());

        public Task<Freezer?> GetByIdAsync(FreezerId id, CancellationToken ct = default) =>
            Task.FromResult(Freezers.FirstOrDefault(f => f.Id == id));

        public Task AddAsync(Freezer freezer, CancellationToken ct = default)
        {
            Freezers.Add(freezer);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Freezer freezer, CancellationToken ct = default) => Task.CompletedTask;

        public Task DeleteAsync(FreezerId id, CancellationToken ct = default)
        {
            Freezers.RemoveAll(f => f.Id == id);
            return Task.CompletedTask;
        }
    }

    private class FakeFreezerItemRepository : IFreezerItemRepository
    {
        public readonly List<FreezerItem> Items = new();
        public readonly List<FreezerTransaction> Transactions = new();

        public Task<IReadOnlyList<FreezerItem>> GetAllAsync(CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<FreezerItem>>(Items.ToList());

        public Task<IReadOnlyList<FreezerItem>> GetByCompartmentIdAsync(FreezerCompartmentId compartmentId, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<FreezerItem>>(Items.Where(i => i.CompartmentId == compartmentId).ToList());

        public Task<FreezerItem?> GetByIdAsync(FreezerItemId id, CancellationToken ct = default) =>
            Task.FromResult(Items.FirstOrDefault(i => i.Id == id));

        public Task AddAsync(FreezerItem item, CancellationToken ct = default)
        {
            Items.Add(item);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(FreezerItem item, CancellationToken ct = default) => Task.CompletedTask;

        public Task DeleteAsync(FreezerItemId id, CancellationToken ct = default)
        {
            Items.RemoveAll(i => i.Id == id);
            return Task.CompletedTask;
        }

        public Task AddTransactionAsync(FreezerTransaction transaction, CancellationToken ct = default)
        {
            Transactions.Add(transaction);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<FreezerTransaction>> GetTransactionsByItemIdAsync(FreezerItemId itemId, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<FreezerTransaction>>(Transactions.Where(t => t.ItemId == itemId).ToList());
    }

    [Fact]
    public async Task CreateFreezerCommand_CreatesAndPersistsFreezer()
    {
        var repo = new FakeFreezerRepository();
        var handler = new CreateFreezerCommandHandler(repo);

        var result = await handler.HandleAsync(new CreateFreezerCommand("Kitchen Freezer", "Kitchen", 1));

        result.Name.Should().Be("Kitchen Freezer");
        repo.Freezers.Should().ContainSingle();
    }

    [Fact]
    public async Task CreateCompartmentCommand_AddsCompartmentToFreezer()
    {
        var repo = new FakeFreezerRepository();
        var freezer = Freezer.Create("Main Freezer");
        repo.Freezers.Add(freezer);

        var handler = new CreateCompartmentCommandHandler(repo);
        var result = await handler.HandleAsync(new CreateCompartmentCommand(freezer.Id.Value, "Drawer A", 0));

        result.Name.Should().Be("Drawer A");
        freezer.Compartments.Should().ContainSingle();
    }

    [Fact]
    public async Task CreateFreezerItemCommand_AddsItemToRepository()
    {
        var itemRepo = new FakeFreezerItemRepository();
        var handler = new CreateFreezerItemCommandHandler(itemRepo);

        var compartmentId = Guid.NewGuid();
        var result = await handler.HandleAsync(new CreateFreezerItemCommand(
            compartmentId,
            "Salmon Fillets",
            FreezerCategory.Seafood,
            4,
            FreezerUnit.Pieces,
            DateTime.UtcNow));

        result.Name.Should().Be("Salmon Fillets");
        result.Quantity.Should().Be(4);
        itemRepo.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task ConsumeFreezerItemCommand_DecrementsItemAndRecordsTransaction()
    {
        var itemRepo = new FakeFreezerItemRepository();
        var item = FreezerItem.Create(
            "Minced Beef",
            FreezerCompartmentId.New(),
            FreezerCategory.MeatAndPoultry,
            3,
            FreezerUnit.Packages,
            DateTime.UtcNow);
        itemRepo.Items.Add(item);

        var handler = new ConsumeFreezerItemCommandHandler(itemRepo);
        var result = await handler.HandleAsync(new ConsumeFreezerItemCommand(
            item.Id.Value,
            1,
            ConsumptionReason.Consumed,
            "Lasagna dinner"));

        result.Quantity.Should().Be(2);
        itemRepo.Transactions.Should().ContainSingle();
        itemRepo.Transactions[0].QuantityChanged.Should().Be(1);
    }

    [Fact]
    public async Task GetFreezerDashboardQuery_AggregatesCountsAndAlerts()
    {
        var freezerRepo = new FakeFreezerRepository();
        var itemRepo = new FakeFreezerItemRepository();

        var freezer = Freezer.Create("Home Freezer");
        var drawer = freezer.AddCompartment("Top Drawer");
        freezerRepo.Freezers.Add(freezer);

        var now = DateTime.UtcNow;
        var expiredItem = FreezerItem.Create("Old Soup", drawer.Id, FreezerCategory.PreparedMeals, 1, FreezerUnit.Portions, now.AddDays(-120), expiryDate: now.AddDays(-10));
        var freshItem = FreezerItem.Create("Frozen Berries", drawer.Id, FreezerCategory.VegetablesAndFruits, 2, FreezerUnit.Packages, now, expiryDate: now.AddDays(90), minimumQuantity: 3);

        itemRepo.Items.Add(expiredItem);
        itemRepo.Items.Add(freshItem);

        var handler = new GetFreezerDashboardQueryHandler(freezerRepo, itemRepo);
        var dashboard = await handler.HandleAsync(new GetFreezerDashboardQuery());

        dashboard.Freezers.Should().ContainSingle();
        dashboard.TotalItemCount.Should().Be(2);
        dashboard.ExpiredCount.Should().Be(1);
        dashboard.LowStockCount.Should().Be(1); // freshItem has quantity 2 <= minimumQuantity 3
        dashboard.UrgentItems.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateFreezerCommand_ModifiesExistingFreezer()
    {
        var repo = new FakeFreezerRepository();
        var freezer = Freezer.Create("Old Name", "Old Location", 1);
        repo.Freezers.Add(freezer);

        var handler = new UpdateFreezerCommandHandler(repo);
        var result = await handler.HandleAsync(new UpdateFreezerCommand(freezer.Id.Value, "New Name", "New Location", 2));

        result.Name.Should().Be("New Name");
        result.Location.Should().Be("New Location");
    }

    [Fact]
    public async Task DeleteFreezerCommand_RemovesFreezer()
    {
        var repo = new FakeFreezerRepository();
        var freezer = Freezer.Create("To Delete");
        repo.Freezers.Add(freezer);

        var handler = new DeleteFreezerCommandHandler(repo);
        var success = await handler.HandleAsync(new DeleteFreezerCommand(freezer.Id.Value));

        success.Should().BeTrue();
        repo.Freezers.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteCompartmentCommand_RemovesCompartmentFromFreezer()
    {
        var repo = new FakeFreezerRepository();
        var freezer = Freezer.Create("Freezer with Compartment");
        var comp = freezer.AddCompartment("Drawer 1");
        repo.Freezers.Add(freezer);

        var handler = new DeleteCompartmentCommandHandler(repo);
        var success = await handler.HandleAsync(new DeleteCompartmentCommand(freezer.Id.Value, comp.Id.Value));

        success.Should().BeTrue();
        freezer.Compartments.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateFreezerItemCommand_UpdatesDetailsAndQuantity()
    {
        var repo = new FakeFreezerItemRepository();
        var compId = FreezerCompartmentId.New();
        var item = FreezerItem.Create("Initial Fish", compId, FreezerCategory.Seafood, 2, FreezerUnit.Pieces, DateTime.UtcNow);
        repo.Items.Add(item);

        var handler = new UpdateFreezerItemCommandHandler(repo);
        var result = await handler.HandleAsync(new UpdateFreezerItemCommand(
            item.Id.Value,
            compId.Value,
            "Updated Cod Fillets",
            FreezerCategory.Seafood,
            5,
            FreezerUnit.Pieces,
            item.FreezeDate,
            item.ExpiryDate,
            1,
            "Freshly frozen"));

        result.Name.Should().Be("Updated Cod Fillets");
        result.Quantity.Should().Be(5);
        result.Notes.Should().Be("Freshly frozen");
    }

    [Fact]
    public async Task DeleteFreezerItemCommand_RemovesItemFromRepository()
    {
        var repo = new FakeFreezerItemRepository();
        var item = FreezerItem.Create("Bread", FreezerCompartmentId.New(), FreezerCategory.Bakery, 1, FreezerUnit.Pieces, DateTime.UtcNow);
        repo.Items.Add(item);

        var handler = new DeleteFreezerItemCommandHandler(repo);
        var success = await handler.HandleAsync(new DeleteFreezerItemCommand(item.Id.Value));

        success.Should().BeTrue();
        repo.Items.Should().BeEmpty();
    }

    [Fact]
    public async Task GetFreezersQuery_ReturnsAllWithCompartments()
    {
        var repo = new FakeFreezerRepository();
        var itemRepo = new FakeFreezerItemRepository();
        var freezer = Freezer.Create("Freezer 1");
        freezer.AddCompartment("Drawer 1");
        repo.Freezers.Add(freezer);

        var handler = new GetFreezersQueryHandler(repo, itemRepo);
        var list = await handler.HandleAsync(new GetFreezersQuery());

        list.Should().ContainSingle();
        list[0].Compartments.Should().ContainSingle();
    }

    [Fact]
    public async Task GetFreezerByIdQuery_ReturnsMatchingFreezer()
    {
        var repo = new FakeFreezerRepository();
        var itemRepo = new FakeFreezerItemRepository();
        var freezer = Freezer.Create("Target Freezer");
        repo.Freezers.Add(freezer);

        var handler = new GetFreezerByIdQueryHandler(repo, itemRepo);
        var found = await handler.HandleAsync(new GetFreezerByIdQuery(freezer.Id.Value));

        found.Should().NotBeNull();
        found!.Name.Should().Be("Target Freezer");
    }

    [Fact]
    public async Task GetFreezerItemsQuery_FiltersByCompartmentAndCategory()
    {
        var repo = new FakeFreezerItemRepository();
        var comp1 = FreezerCompartmentId.New();
        var comp2 = FreezerCompartmentId.New();

        repo.Items.Add(FreezerItem.Create("Beef", comp1, FreezerCategory.MeatAndPoultry, 1, FreezerUnit.Pieces, DateTime.UtcNow));
        repo.Items.Add(FreezerItem.Create("Chicken", comp1, FreezerCategory.MeatAndPoultry, 2, FreezerUnit.Pieces, DateTime.UtcNow));
        repo.Items.Add(FreezerItem.Create("Salmon", comp2, FreezerCategory.Seafood, 1, FreezerUnit.Pieces, DateTime.UtcNow));

        var handler = new GetFreezerItemsQueryHandler(repo);

        var comp1Items = await handler.HandleAsync(new GetFreezerItemsQuery(comp1.Value, null));
        comp1Items.Should().HaveCount(2);

        var seafoodItems = await handler.HandleAsync(new GetFreezerItemsQuery(null, FreezerCategory.Seafood));
        seafoodItems.Should().ContainSingle();
        seafoodItems[0].Name.Should().Be("Salmon");
    }
}
