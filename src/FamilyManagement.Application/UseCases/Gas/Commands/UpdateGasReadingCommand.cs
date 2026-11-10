using FamilyManagement.Application.UseCases.Gas.DTOs;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Gas.Commands;

public record UpdateGasReadingCommand(
    Guid Id,
    DateTime RecordedDate,
    decimal ReadingM3,
    string? Notes = null);

public class UpdateGasReadingCommandHandler
{
    private readonly IGasReadingRepository _readingRepository;
    private readonly IGasPricingRepository _pricingRepository;

    public UpdateGasReadingCommandHandler(
        IGasReadingRepository readingRepository,
        IGasPricingRepository pricingRepository)
    {
        _readingRepository = readingRepository;
        _pricingRepository = pricingRepository;
    }

    public async Task<GasReadingDto> HandleAsync(UpdateGasReadingCommand command, CancellationToken ct = default)
    {
        var id = GasReadingId.From(command.Id);
        var reading = await _readingRepository.GetByIdAsync(id, ct);
        if (reading == null)
        {
            throw new KeyNotFoundException($"Gas reading with ID '{command.Id}' was not found.");
        }

        reading.Update(command.RecordedDate, command.ReadingM3, command.Notes);
        await _readingRepository.UpdateAsync(reading, ct);

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
