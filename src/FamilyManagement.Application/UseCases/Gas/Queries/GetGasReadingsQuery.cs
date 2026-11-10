using FamilyManagement.Application.UseCases.Gas.DTOs;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Gas.Queries;

public record GetGasReadingsQuery;

public class GetGasReadingsQueryHandler
{
    private readonly IGasReadingRepository _readingRepository;
    private readonly IGasPricingRepository _pricingRepository;

    public GetGasReadingsQueryHandler(
        IGasReadingRepository readingRepository,
        IGasPricingRepository pricingRepository)
    {
        _readingRepository = readingRepository;
        _pricingRepository = pricingRepository;
    }

    public async Task<List<GasReadingDto>> HandleAsync(GetGasReadingsQuery query, CancellationToken ct = default)
    {
        var readings = await _readingRepository.GetAllAsync(ct);
        var ordered = readings.OrderBy(r => r.RecordedDate).ToList();
        var settings = await _pricingRepository.GetSettingsAsync(ct);
        var snapshots = await _pricingRepository.GetAllSnapshotsAsync(ct);
        var snapshotMap = snapshots.ToDictionary(s => (s.Year, s.Month), s => s.TotalPricePerM3);

        var result = new List<GasReadingDto>();
        for (int i = 0; i < ordered.Count; i++)
        {
            var current = ordered[i];
            decimal? deltaM3 = null;
            decimal? cost = null;

            if (i > 0)
            {
                var prev = ordered[i - 1];
                deltaM3 = Math.Max(0m, current.ReadingM3 - prev.ReadingM3);
                var rate = snapshotMap.GetValueOrDefault((current.RecordedDate.Year, current.RecordedDate.Month), settings.TotalPricePerM3);
                cost = Math.Round(deltaM3.Value * rate, 2);
            }

            result.Add(new GasReadingDto(
                current.Id.Value,
                current.RecordedDate,
                current.ReadingM3,
                deltaM3,
                cost,
                current.Notes,
                current.CreatedAt));
        }

        // Return latest first for presentation in tables
        result.Reverse();
        return result;
    }
}
