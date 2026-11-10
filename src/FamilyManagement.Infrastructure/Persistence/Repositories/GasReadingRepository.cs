using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FamilyManagement.Infrastructure.Persistence.Repositories;

public class GasReadingRepository : IGasReadingRepository
{
    private readonly ReceiptDbContext _context;

    public GasReadingRepository(ReceiptDbContext context) => _context = context;

    public async Task<GasReading?> GetByIdAsync(GasReadingId id, CancellationToken ct = default)
        => await _context.GasReadings.FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IReadOnlyList<GasReading>> GetAllAsync(CancellationToken ct = default)
        => await _context.GasReadings.OrderBy(r => r.RecordedDate).ToListAsync(ct);

    public async Task AddAsync(GasReading reading, CancellationToken ct = default)
    {
        await _context.GasReadings.AddAsync(reading, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(GasReading reading, CancellationToken ct = default)
    {
        _context.GasReadings.Update(reading);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(GasReading reading, CancellationToken ct = default)
    {
        _context.GasReadings.Remove(reading);
        await _context.SaveChangesAsync(ct);
    }
}
