using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Receipts.Queries;

public record GetDistinctMerchantsQuery;

public class GetDistinctMerchantsQueryHandler
{
    private readonly IReceiptRepository _repository;

    public GetDistinctMerchantsQueryHandler(IReceiptRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<string>> HandleAsync(GetDistinctMerchantsQuery query, CancellationToken cancellationToken = default)
    {
        return await _repository.GetDistinctMerchantsAsync(cancellationToken);
    }
}
