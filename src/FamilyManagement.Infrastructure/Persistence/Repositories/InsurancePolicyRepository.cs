using FamilyManagement.Domain.Entities;
using FamilyManagement.Domain.Enums;
using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FamilyManagement.Infrastructure.Persistence.Repositories;

public class InsurancePolicyRepository : IInsurancePolicyRepository
{
    private readonly ReceiptDbContext _dbContext;

    public InsurancePolicyRepository(ReceiptDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(InsurancePolicy policy, CancellationToken cancellationToken = default)
    {
        await _dbContext.InsurancePolicies.AddAsync(policy, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<InsurancePolicy?> GetByIdAsync(InsurancePolicyId id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.InsurancePolicies
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<List<InsurancePolicy>> ListAsync(
        InsuranceCategory? category = null,
        InsurancePolicyStatus? status = null,
        string? insurer = null,
        CancellationToken cancellationToken = default)
    {
        var query = _dbContext.InsurancePolicies.AsQueryable();

        if (category.HasValue)
        {
            query = query.Where(p => p.Category == category.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(insurer))
        {
            var trimmed = insurer.Trim().ToLower();
            query = query.Where(p => p.Insurer.ToLower().Contains(trimmed));
        }

        return await query
            .OrderBy(p => p.Category)
            .ThenBy(p => p.Insurer)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateAsync(InsurancePolicy policy, CancellationToken cancellationToken = default)
    {
        _dbContext.InsurancePolicies.Update(policy);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(InsurancePolicy policy, CancellationToken cancellationToken = default)
    {
        _dbContext.InsurancePolicies.Remove(policy);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
