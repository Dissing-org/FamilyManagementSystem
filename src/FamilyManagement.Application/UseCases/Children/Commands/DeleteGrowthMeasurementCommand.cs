using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Commands;

/// <summary>
/// Command to delete a growth measurement.
/// </summary>
/// <param name="Id">The unique identifier of the growth measurement to delete.</param>
public record DeleteGrowthMeasurementCommand(Guid Id);

/// <summary>
/// Handler for deleting a growth measurement.
/// </summary>
public class DeleteGrowthMeasurementCommandHandler
{
    private readonly IGrowthMeasurementRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteGrowthMeasurementCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The growth measurement repository.</param>
    public DeleteGrowthMeasurementCommandHandler(IGrowthMeasurementRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles deleting a growth measurement by its identifier.
    /// </summary>
    /// <param name="command">The command containing the identifier of the measurement to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning <c>true</c> if deleted; otherwise, <c>false</c>.</returns>
    public async Task<bool> HandleAsync(
        DeleteGrowthMeasurementCommand command,
        CancellationToken cancellationToken = default)
    {
        var measurement = await _repository.GetByIdAsync(GrowthMeasurementId.From(command.Id), cancellationToken);
        if (measurement is null)
        {
            return false;
        }

        await _repository.DeleteAsync(measurement, cancellationToken);
        return true;
    }
}
