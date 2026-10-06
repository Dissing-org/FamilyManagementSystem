using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Children.Commands;

public record DeleteGrowthMeasurementCommand(Guid Id);

public class DeleteGrowthMeasurementCommandHandler
{
    private readonly IGrowthMeasurementRepository _repository;

    public DeleteGrowthMeasurementCommandHandler(IGrowthMeasurementRepository repository)
    {
        _repository = repository;
    }

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
