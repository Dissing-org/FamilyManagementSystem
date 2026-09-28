using FamilyManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FamilyManagement.Infrastructure.Persistence;

public class ReceiptDbContext : DbContext
{
    public DbSet<Receipt> Receipts => Set<Receipt>();

    public ReceiptDbContext(DbContextOptions<ReceiptDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReceiptDbContext).Assembly);
    }
}
