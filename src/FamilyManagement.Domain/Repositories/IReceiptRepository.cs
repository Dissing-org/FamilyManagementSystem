using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

public interface IReceiptRepository
{
    Task<Receipt?> GetByIdAsync(ReceiptId id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Receipt>> ListAsync(string? merchantFilter = null, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetDistinctMerchantsAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Receipt receipt, CancellationToken cancellationToken = default);
    Task UpdateAsync(Receipt receipt, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
