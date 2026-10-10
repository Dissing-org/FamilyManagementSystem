using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.Vehicles;

public record DeleteMileageLogCommand(Guid Id);

public class DeleteMileageLogCommandHandler
{
    private readonly IMileageLogRepository _repository;

    public DeleteMileageLogCommandHandler(IMileageLogRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(DeleteMileageLogCommand command, CancellationToken cancellationToken = default)
    {
        var log = await _repository.GetByIdAsync(MileageLogId.From(command.Id), cancellationToken);
        if (log is null)
        {
            return false;
        }

        await _repository.DeleteAsync(log, cancellationToken);
        return true;
    }
}
