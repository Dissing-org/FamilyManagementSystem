using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FamilyManagement.Infrastructure.Persistence.Repositories;

public class GasPricingRepository : IGasPricingRepository
{
    private readonly ReceiptDbContext _context;

    public GasPricingRepository(ReceiptDbContext context) => _context = context;

    public async Task<GasPricingSettings> GetSettingsAsync(CancellationToken ct = default)
    {
        var settings = await _context.GasPricingSettings.FirstOrDefaultAsync(ct);
        if (settings == null)
        {
            settings = new GasPricingSettings();
            _context.GasPricingSettings.Add(settings);
            await _context.SaveChangesAsync(ct);
        }
        return settings;
    }

    public async Task SaveSettingsAsync(GasPricingSettings settings, CancellationToken ct = default)
    {
        _context.GasPricingSettings.Update(settings);
        await _context.SaveChangesAsync(ct);
    }

    public async Task<GasPriceSnapshot?> GetSnapshotAsync(int year, int month, CancellationToken ct = default)
        => await _context.GasPriceSnapshots.FirstOrDefaultAsync(s => s.Year == year && s.Month == month, ct);

    public async Task<IReadOnlyList<GasPriceSnapshot>> GetAllSnapshotsAsync(CancellationToken ct = default)
        => await _context.GasPriceSnapshots.OrderBy(s => s.Year).ThenBy(s => s.Month).ToListAsync(ct);

    public async Task SaveSnapshotAsync(GasPriceSnapshot snapshot, CancellationToken ct = default)
    {
        var existing = await GetSnapshotAsync(snapshot.Year, snapshot.Month, ct);
        if (existing != null)
        {
            _context.GasPriceSnapshots.Remove(existing);
        }
        _context.GasPriceSnapshots.Add(snapshot);
        await _context.SaveChangesAsync(ct);
    }
}
