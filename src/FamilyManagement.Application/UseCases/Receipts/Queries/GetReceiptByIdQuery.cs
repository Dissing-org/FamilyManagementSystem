using FamilyManagement.Application.DTOs;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Receipts.Queries;

public record GetReceiptByIdQuery(Guid Id);

public class GetReceiptByIdQueryHandler
{
    private readonly IReceiptRepository _repository;

    public GetReceiptByIdQueryHandler(IReceiptRepository repository)
    {
        _repository = repository;
    }

    public async Task<ReceiptResponseDto?> HandleAsync(GetReceiptByIdQuery query, CancellationToken cancellationToken = default)
    {
        var receipt = await _repository.GetByIdAsync(ReceiptId.From(query.Id), cancellationToken);
        return receipt is null ? null : ReceiptResponseDto.FromEntity(receipt);
    }
}
