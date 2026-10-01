using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Insurances.Commands;

public record DeleteInsurancePolicyCommand(Guid Id);

public class DeleteInsurancePolicyCommandHandler
{
    private readonly IInsurancePolicyRepository _repository;

    public DeleteInsurancePolicyCommandHandler(IInsurancePolicyRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(DeleteInsurancePolicyCommand command, CancellationToken cancellationToken = default)
    {
        var policy = await _repository.GetByIdAsync(InsurancePolicyId.From(command.Id), cancellationToken);
        if (policy is null)
        {
            return false;
        }

        await _repository.DeleteAsync(policy, cancellationToken);
        return true;
    }
}
