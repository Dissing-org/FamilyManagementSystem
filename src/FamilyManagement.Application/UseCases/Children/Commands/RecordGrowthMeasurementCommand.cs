using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Commands;

public record RecordGrowthMeasurementCommand(
    Guid ChildId,
    DateTime RecordedDate,
    decimal? HeightCm,
    decimal? WeightKg,
    decimal? HeadCircumferenceCm,
    string? Notes = null);

public class RecordGrowthMeasurementCommandHandler
{
    private readonly IGrowthMeasurementRepository _repository;
    private readonly IChildProfileRepository _childRepository;

    public RecordGrowthMeasurementCommandHandler(
        IGrowthMeasurementRepository repository,
        IChildProfileRepository childRepository)
    {
        _repository = repository;
        _childRepository = childRepository;
    }

    public async Task<GrowthMeasurementDto> HandleAsync(
        RecordGrowthMeasurementCommand command,
        CancellationToken cancellationToken = default)
    {
        var child = await _childRepository.GetByIdAsync(ChildId.From(command.ChildId), cancellationToken);
        if (child is null)
        {
            throw new KeyNotFoundException($"Child with ID '{command.ChildId}' was not found.");
        }

        var measurement = GrowthMeasurement.Create(
            child.Id,
            command.RecordedDate,
            command.HeightCm,
            command.WeightKg,
            command.HeadCircumferenceCm,
            command.Notes);

        await _repository.AddAsync(measurement, cancellationToken);

        return GrowthMeasurementDto.FromDomain(measurement);
    }
}
