using FamilyManagement.Domain.Repositories;
using FamilyManagement.Domain.ValueObjects;

namespace FamilyManagement.Application.UseCases.Insurances.Commands;

/// <summary>
/// Command to delete an insurance policy by its unique identifier.
/// </summary>
public record DeleteInsurancePolicyCommand(Guid Id);

/// <summary>
/// Handler for deleting an insurance policy.
/// </summary>
public class DeleteInsurancePolicyCommandHandler
{
    private readonly IInsurancePolicyRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteInsurancePolicyCommandHandler"/> class.
    /// </summary>
    /// <param name="repository">The insurance policy repository.</param>
    public DeleteInsurancePolicyCommandHandler(IInsurancePolicyRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Handles the deletion of an insurance policy.
    /// </summary>
    /// <param name="command">The command containing the identifier of the policy to delete.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns><see langword="true"/> if the insurance policy was found and deleted; otherwise, <see langword="false"/>.</returns>
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
