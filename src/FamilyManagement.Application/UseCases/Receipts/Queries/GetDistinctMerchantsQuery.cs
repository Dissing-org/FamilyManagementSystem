using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Receipts.Queries;

/// <summary>
/// Query to retrieve all distinct merchant names from recorded receipts.
/// </summary>
public record GetDistinctMerchantsQuery;

/// <summary>
/// Handler for retrieving distinct merchant names from receipts.
/// </summary>
public class GetDistinctMerchantsQueryHandler
{
    private readonly IReceiptRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetDistinctMerchantsQueryHandler"/> class.
    /// </summary>
    /// <param name="repository">The receipt repository.</param>
    public GetDistinctMerchantsQueryHandler(IReceiptRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles retrieving unique merchant names across all receipts.
    /// </summary>
    /// <param name="query">The distinct merchants query.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, returning a read-only list of distinct merchant names.</returns>
    public async Task<IReadOnlyList<string>> HandleAsync(GetDistinctMerchantsQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetDistinctMerchantsAsync(cancellationToken);
    }
}
