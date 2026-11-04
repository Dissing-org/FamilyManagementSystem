using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Receipts.Commands;

/// <summary>
/// Command to delete (archive) a receipt.
/// </summary>
/// <param name="Id">The unique identifier of the receipt to delete.</param>
public record DeleteReceiptCommand(Guid Id);

/// <summary>
/// Handler for deleting (archiving) a receipt.
/// </summary>
public class DeleteReceiptCommandHandler
{
    private readonly IReceiptRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteReceiptCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The receipt repository.</param>
    public DeleteReceiptCommandHandler(IReceiptRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles archiving an existing receipt by its identifier.
    /// </summary>
    /// <param name="command">The command containing the receipt identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning <c>true</c> if found and archived; otherwise, <c>false</c>.</returns>
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
