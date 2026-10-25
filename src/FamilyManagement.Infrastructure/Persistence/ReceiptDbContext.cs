using FamilyManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FamilyManagement.Infrastructure.Persistence;

public class ReceiptDbContext : DbContext
{
    public DbSet<Receipt> Receipts => Set<Receipt>();
    public DbSet<InsurancePolicy> InsurancePolicies => Set<InsurancePolicy>();
    public DbSet<ChildProfile> ChildProfiles => Set<ChildProfile>();
    public DbSet<GrowthMeasurement> GrowthMeasurements => Set<GrowthMeasurement>();
    public DbSet<ChildMilestone> ChildMilestones => Set<ChildMilestone>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<MileageLogEntry> MileageLogs => Set<MileageLogEntry>();
    public DbSet<VehicleServiceRecord> VehicleServices => Set<VehicleServiceRecord>();
    public DbSet<Freezer> Freezers => Set<Freezer>();
    public DbSet<FreezerCompartment> FreezerCompartments => Set<FreezerCompartment>();
    public DbSet<FreezerItem> FreezerItems => Set<FreezerItem>();
    public DbSet<FreezerTransaction> FreezerTransactions => Set<FreezerTransaction>();
    public DbSet<FreezerCatalogItem> FreezerCatalogItems => Set<FreezerCatalogItem>();

    public ReceiptDbContext(DbContextOptions<ReceiptDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReceiptDbContext).Assembly);
    }
}
