using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Receipts.Commands;

public record DeleteReceiptCommand(Guid Id);

public class DeleteReceiptCommandHandler
{
    private readonly IReceiptRepository _repository;

    public DeleteReceiptCommandHandler(IReceiptRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(DeleteReceiptCommand command, CancellationToken cancellationToken = default)
    {
        var receipt = await _repository.GetByIdAsync(ReceiptId.From(command.Id), cancellationToken);
        if (receipt is null)
        {
            return false;
        }

        receipt.Archive();
        await _repository.UpdateAsync(receipt, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);
        return true;
    }
}
