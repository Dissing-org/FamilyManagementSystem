using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FamilyManagement.Infrastructure.Persistence.Repositories;

public class ReceiptRepository : IReceiptRepository
{
    private readonly ReceiptDbContext _context;

    public ReceiptRepository(ReceiptDbContext context)
    {
        _context = context;
    }

    public async Task<Receipt?> GetByIdAsync(ReceiptId id, CancellationToken cancellationToken = default)
    {
        return await _context.Receipts
            .FirstOrDefaultAsync(r => r.Id == id && r.Status == ReceiptStatus.Active, cancellationToken);
    }

    public async Task<IReadOnlyList<Receipt>> ListAsync(string? merchantFilter = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Receipts.Where(r => r.Status == ReceiptStatus.Active);

        if (!string.IsNullOrWhiteSpace(merchantFilter))
        {
            query = query.Where(r => EF.Functions.Like(r.Merchant, $"%{merchantFilter.Trim()}%"));
        }

        return await query.OrderByDescending(r => r.PurchaseDate).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Receipt receipt, CancellationToken cancellationToken = default)
    {
        await _context.Receipts.AddAsync(receipt, cancellationToken);
    }

    public Task UpdateAsync(Receipt receipt, CancellationToken cancellationToken = default)
    {
        _context.Receipts.Update(receipt);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
