using FamilyManagement.Application.DTOs;
using FamilyManagement.Domain.Repositories;

namespace FamilyManagement.Application.UseCases.Receipts.Queries;

public record ListReceiptsQuery(string? MerchantFilter = null);

public class ListReceiptsQueryHandler
{
    private readonly IReceiptRepository _repository;

    public ListReceiptsQueryHandler(IReceiptRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<ReceiptResponseDto>> HandleAsync(ListReceiptsQuery query, CancellationToken cancellationToken = default)
    {
        var receipts = await _repository.ListAsync(query.MerchantFilter, cancellationToken);
        return receipts.Select(ReceiptResponseDto.FromEntity).ToList();
    }
}
