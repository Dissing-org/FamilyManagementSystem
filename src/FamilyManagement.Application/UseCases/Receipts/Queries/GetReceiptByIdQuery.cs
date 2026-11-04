using FamilyManagement.Application.DTOs;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Receipts.Queries;

/// <summary>
/// Query to retrieve a receipt by its unique identifier.
/// </summary>
/// <param name="Id">The unique identifier of the receipt.</param>
public record GetReceiptByIdQuery(Guid Id);

/// <summary>
/// Handler for retrieving a receipt by its unique identifier.
/// </summary>
public class GetReceiptByIdQueryHandler
{
    private readonly IReceiptRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetReceiptByIdQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The receipt repository.</param>
    public GetReceiptByIdQueryHandler(IReceiptRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles retrieving a receipt by its identifier.
    /// </summary>
    /// <param name="query">The query containing the receipt identifier.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning the <see cref="ReceiptResponseDto"/> if found; otherwise, <c>null</c>.</returns>
    public async Task<ReceiptResponseDto?> HandleAsync(GetReceiptByIdQuery query, CancellationToken cancellationToken = default)
    {
        var receipt = await _repository.GetByIdAsync(ReceiptId.From(query.Id), cancellationToken);
        return receipt is null ? null : ReceiptResponseDto.FromEntity(receipt);
    }
}
