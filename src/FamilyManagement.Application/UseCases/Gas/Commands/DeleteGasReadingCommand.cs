using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Gas.Commands;

public record DeleteGasReadingCommand(Guid Id);

public class DeleteGasReadingCommandHandler
{
    private readonly IGasReadingRepository _readingRepository;

    public DeleteGasReadingCommandHandler(IGasReadingRepository readingRepository)
    {
        _readingRepository = readingRepository;
    }

    public async Task HandleAsync(DeleteGasReadingCommand command, CancellationToken ct = default)
    {
        var id = GasReadingId.From(command.Id);
        var reading = await _readingRepository.GetByIdAsync(id, ct);
        if (reading == null)
        {
            throw new KeyNotFoundException($"Gas reading with ID '{command.Id}' was not found.");
        }

        await _readingRepository.DeleteAsync(reading, ct);
    }
}
