using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FamilyManagement.Infrastructure.Persistence.Repositories;

public class FreezerRepository : IFreezerRepository
{
    private readonly ReceiptDbContext _context;

    public FreezerRepository(ReceiptDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Freezer>> GetAllWithCompartmentsAsync(CancellationToken ct = default)
    {
        return await _context.Freezers
            .Include(f => f.Compartments)
            .OrderBy(f => f.DisplayOrder)
            .ToListAsync(ct);
    }

    public async Task<Freezer?> GetByIdAsync(FreezerId id, CancellationToken ct = default)
    {
        return await _context.Freezers
            .Include(f => f.Compartments)
            .FirstOrDefaultAsync(f => f.Id == id, ct);
    }

    public async Task AddAsync(Freezer freezer, CancellationToken ct = default)
    {
        await _context.Freezers.AddAsync(freezer, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Freezer freezer, CancellationToken ct = default)
    {
        _context.Freezers.Update(freezer);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(FreezerId id, CancellationToken ct = default)
    {
        var freezer = await _context.Freezers
            .Include(f => f.Compartments)
            .FirstOrDefaultAsync(f => f.Id == id, ct);

        if (freezer is not null)
        {
            var compartmentIds = freezer.Compartments.Select(c => c.Id).ToList();
            if (compartmentIds.Count > 0)
            {
                var items = await _context.FreezerItems
                    .Where(i => compartmentIds.Contains(i.CompartmentId))
                    .ToListAsync(ct);

                if (items.Count > 0)
                {
                    var itemIds = items.Select(i => i.Id).ToList();
                    var transactions = await _context.FreezerTransactions
                        .Where(t => itemIds.Contains(t.ItemId))
                        .ToListAsync(ct);

                    if (transactions.Count > 0)
                    {
                        _context.FreezerTransactions.RemoveRange(transactions);
                    }

                    _context.FreezerItems.RemoveRange(items);
                }
            }

            _context.Freezers.Remove(freezer);
            await _context.SaveChangesAsync(ct);
        }
    }
}
