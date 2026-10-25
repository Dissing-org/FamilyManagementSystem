using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FamilyManagement.Infrastructure.Persistence.Repositories;

public class FreezerCatalogRepository : IFreezerCatalogRepository
{
    private readonly ReceiptDbContext _context;

    public FreezerCatalogRepository(ReceiptDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<FreezerCatalogItem>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.FreezerCatalogItems
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .ToListAsync(ct);
    }

    public async Task<FreezerCatalogItem?> GetByNameAsync(string name, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return null;
        }

        var normalized = name.Trim().ToUpperInvariant();
        return await _context.FreezerCatalogItems
            .FirstOrDefaultAsync(c => c.NormalizedName == normalized, ct);
    }

    public async Task<FreezerCatalogItem> AddAsync(FreezerCatalogItem item, CancellationToken ct = default)
    {
        await _context.FreezerCatalogItems.AddAsync(item, ct);
        try
        {
            await _context.SaveChangesAsync(ct);
            return item;
        }
        catch (DbUpdateException)
        {
            _context.Entry(item).State = EntityState.Detached;
            throw;
        }
    }

    public async Task UpdateAsync(FreezerCatalogItem item, CancellationToken ct = default)
    {
        _context.FreezerCatalogItems.Update(item);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(FreezerCatalogItemId id, CancellationToken ct = default)
    {
        var existing = await _context.FreezerCatalogItems
            .FirstOrDefaultAsync(c => c.Id == id, ct);
        if (existing is not null)
        {
            _context.FreezerCatalogItems.Remove(existing);
            await _context.SaveChangesAsync(ct);
        }
    }
}
