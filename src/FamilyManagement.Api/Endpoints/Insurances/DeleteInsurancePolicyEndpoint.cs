using FastEndpoints;
using FamilyManagement.Application.UseCases.Insurances.Commands;

namespace FamilyManagement.Api.Endpoints.Insurances;

public class DeleteInsurancePolicyRequest
{
    public Guid Id { get; set; }
}

public class DeleteInsurancePolicyEndpoint : Endpoint<DeleteInsurancePolicyRequest>
{
    private readonly DeleteInsurancePolicyCommandHandler _handler;

    public DeleteInsurancePolicyEndpoint(DeleteInsurancePolicyCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Delete("/api/insurances/{id}");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Delete an insurance policy";
            s.Description = "Removes an insurance policy from the system.";
        });
    }

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
