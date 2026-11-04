using FamilyManagement.Application.DTOs;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Receipts.Queries;

/// <summary>
/// Query to list receipts, with an optional merchant filter.
/// </summary>
/// <param name="MerchantFilter">The optional merchant name filter.</param>
public record ListReceiptsQuery(string? MerchantFilter = null);

/// <summary>
/// Handler for listing receipts.
/// </summary>
public class ListReceiptsQueryHandler
{
    private readonly IReceiptRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ListReceiptsQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The receipt repository.</param>
    public ListReceiptsQueryHandler(IReceiptRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles listing receipts, optionally filtered by merchant name.
    /// </summary>
    /// <param name="query">The query containing optional filter criteria.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a read-only list of <see cref="ReceiptResponseDto"/> instances.</returns>
    public async Task<IReadOnlyList<ReceiptResponseDto>> HandleAsync(ListReceiptsQuery query, CancellationToken cancellationToken = default)
    {
        var receipts = await _repository.ListAsync(query.MerchantFilter, cancellationToken);
        return receipts.Select(ReceiptResponseDto.FromEntity).ToList();
    }
}
