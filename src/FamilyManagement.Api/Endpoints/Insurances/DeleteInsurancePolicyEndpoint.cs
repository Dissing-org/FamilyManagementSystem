using FastEndpoints;
using FamilyManagement.Application.UseCases.Insurances.Commands;

namespace FamilyManagement.Api.Endpoints.Insurances;

public class DeleteInsurancePolicyRequest
{
    public Guid Id { get; set; }
}

/// <summary>
/// Endpoint for deleting an insurance policy.
/// </summary>
public class DeleteInsurancePolicyEndpoint : Endpoint<DeleteInsurancePolicyRequest>
{
    private readonly DeleteInsurancePolicyCommandHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="DeleteInsurancePolicyEndpoint"/> class.
    /// </summary>
    /// <param name="handler">The command handler for deleting insurance policies.</param>
    public DeleteInsurancePolicyEndpoint(DeleteInsurancePolicyCommandHandler handler)
    {
        _handler = handler;
    }

    /// <summary>
    /// Configures the route and OpenAPI metadata for deleting an insurance policy.
    /// </summary>
    public override void Configure()
    {
        Delete("/api/insurances/{id}");
        Summary(s =>
        {
            s.Summary = "Delete an insurance policy";
            s.Description = "Removes an insurance policy from the system.";
        });
    }

    /// <summary>
    /// Handles the HTTP DELETE request to remove an insurance policy.
    /// </summary>
    /// <param name="req">The request parameters containing the policy identifier.</param>
    /// <param name="ct">The cancellation token to observe.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public override async Task HandleAsync(DeleteInsurancePolicyRequest req, CancellationToken ct)
    {
        var deleted = await _handler.HandleAsync(new DeleteInsurancePolicyCommand(req.Id), ct);
        if (!deleted)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.NotFound());
            return;
        }

        await HttpContext.Response.SendResultAsync(TypedResults.NoContent());
    }
}
