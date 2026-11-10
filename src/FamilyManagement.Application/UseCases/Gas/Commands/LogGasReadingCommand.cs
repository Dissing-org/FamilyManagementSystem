using FamilyManagement.Application.UseCases.Gas.DTOs;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Gas.Commands;

public record LogGasReadingCommand(
    DateTime RecordedDate,
    decimal ReadingM3,
    string? Notes = null);

public class LogGasReadingCommandHandler
{
    private readonly IGasReadingRepository _readingRepository;
    private readonly IGasPricingRepository _pricingRepository;

    public LogGasReadingCommandHandler(
        IGasReadingRepository readingRepository,
        IGasPricingRepository pricingRepository)
    {
        _readingRepository = readingRepository;
        _pricingRepository = pricingRepository;
    }

    public async Task<GasReadingDto> HandleAsync(LogGasReadingCommand command, CancellationToken ct = default)
    {
        var reading = GasReading.Create(command.RecordedDate, command.ReadingM3, command.Notes);
        await _readingRepository.AddAsync(reading, ct);

        var allReadings = await _readingRepository.GetAllAsync(ct);
        var ordered = allReadings.OrderBy(r => r.RecordedDate).ToList();
        var index = ordered.FindIndex(r => r.Id == reading.Id);

        decimal? deltaM3 = null;
        if (index > 0)
        {
            var prev = ordered[index - 1];
            deltaM3 = Math.Max(0m, reading.ReadingM3 - prev.ReadingM3);
        }

        var settings = await _pricingRepository.GetSettingsAsync(ct);
        decimal? estimatedCost = deltaM3.HasValue ? Math.Round(deltaM3.Value * settings.TotalPricePerM3, 2) : null;

        return new GasReadingDto(
            reading.Id.Value,
            reading.RecordedDate,
            reading.ReadingM3,
            deltaM3,
            estimatedCost,
            reading.Notes,
            reading.CreatedAt);
    }
}
