using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Domain.Repositories;

public interface IChildProfileRepository
{
    Task AddAsync(ChildProfile child, CancellationToken cancellationToken = default);
    Task<ChildProfile?> GetByIdAsync(ChildId id, CancellationToken cancellationToken = default);
    Task<List<ChildProfile>> ListAllAsync(CancellationToken cancellationToken = default);
    Task UpdateAsync(ChildProfile child, CancellationToken cancellationToken = default);
    Task DeleteAsync(ChildProfile child, CancellationToken cancellationToken = default);
}
