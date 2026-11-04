using FamilyManagement.Application.DTOs.Children;
using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Commands;

/// <summary>
/// Command to record a growth measurement for a child.
/// </summary>
/// <param name="ChildId">The unique identifier of the child.</param>
/// <param name="RecordedDate">The date when the measurement was taken.</param>
/// <param name="HeightCm">The height in centimeters, if measured.</param>
/// <param name="WeightKg">The weight in kilograms, if measured.</param>
/// <param name="HeadCircumferenceCm">The head circumference in centimeters, if measured.</param>
/// <param name="Notes">Optional notes regarding the measurement.</param>
public record RecordGrowthMeasurementCommand(
    Guid ChildId,
    DateTime RecordedDate,
    decimal? HeightCm,
    decimal? WeightKg,
    decimal? HeadCircumferenceCm,
    string? Notes = null);

/// <summary>
/// Handler for recording a growth measurement for a child.
/// </summary>
public class RecordGrowthMeasurementCommandHandler
{
    private readonly IGrowthMeasurementRepository _repository;
    private readonly IChildProfileRepository _childRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RecordGrowthMeasurementCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The growth measurement repository.</param>
    /// <param name="childRepository">The child profile repository.</param>
    public RecordGrowthMeasurementCommandHandler(
        IGrowthMeasurementRepository repository,
        IChildProfileRepository childRepository)
    {
        _repository = repository;
        _childRepository = childRepository;
    }

    /// <summary>
    /// Handles recording and persisting a new growth measurement for a child.
    /// </summary>
    /// <param name="command">The command containing the measurement data.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the recorded <see cref="GrowthMeasurementDto"/>.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the child with the specified ID is not found.</exception>
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
