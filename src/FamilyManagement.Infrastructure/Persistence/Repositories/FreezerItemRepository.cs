using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FamilyManagement.Infrastructure.Persistence.Repositories;

public class FreezerItemRepository : IFreezerItemRepository
{
    private readonly ReceiptDbContext _context;

    public FreezerItemRepository(ReceiptDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<FreezerItem>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.FreezerItems
            .OrderBy(i => i.ExpiryDate)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<FreezerItem>> GetByCompartmentIdAsync(FreezerCompartmentId compartmentId, CancellationToken ct = default)
    {
        return await _context.FreezerItems
            .Where(i => i.CompartmentId == compartmentId)
            .OrderBy(i => i.ExpiryDate)
            .ToListAsync(ct);
    }

    public async Task<FreezerItem?> GetByIdAsync(FreezerItemId id, CancellationToken ct = default)
    {
        return await _context.FreezerItems
            .FirstOrDefaultAsync(i => i.Id == id, ct);
    }

    public async Task AddAsync(FreezerItem item, CancellationToken ct = default)
    {
        await _context.FreezerItems.AddAsync(item, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(FreezerItem item, CancellationToken ct = default)
    {
        _context.FreezerItems.Update(item);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(FreezerItemId id, CancellationToken ct = default)
    {
        var item = await _context.FreezerItems.FirstOrDefaultAsync(i => i.Id == id, ct);
        if (item is not null)
        {
            var transactions = await _context.FreezerTransactions
                .Where(t => t.ItemId == id)
                .ToListAsync(ct);

            if (transactions.Count > 0)
            {
                _context.FreezerTransactions.RemoveRange(transactions);
            }

            _context.FreezerItems.Remove(item);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task AddTransactionAsync(FreezerTransaction transaction, CancellationToken ct = default)
    {
        await _context.FreezerTransactions.AddAsync(transaction, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<FreezerTransaction>> GetTransactionsByItemIdAsync(FreezerItemId itemId, CancellationToken ct = default)
    {
        return await _context.FreezerTransactions
            .Where(t => t.ItemId == itemId)
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync(ct);
    }
}
