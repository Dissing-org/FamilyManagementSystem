using FastEndpoints;
using FamilyManagement.Application.Interfaces;

namespace FamilyManagement.Api.Endpoints.Auth;

public class ResetDemoDataResponse
{
    public string Message { get; set; } = string.Empty;
    public DateTime ResetAt { get; set; }
}

public class ResetDemoDataEndpoint : EndpointWithoutRequest<ResetDemoDataResponse>
{
    private readonly ITenantContext _tenantContext;
    private readonly IDemoDataSeeder _demoDataSeeder;

    public ResetDemoDataEndpoint(ITenantContext tenantContext, IDemoDataSeeder demoDataSeeder)
    {
        _tenantContext = tenantContext;
        _demoDataSeeder = demoDataSeeder;
    }

    public override void Configure()
    {
        Post("/api/demo/reset");
        Summary(s =>
        {
            s.Summary = "Reset the demo sandbox database and receipt files";
            s.Description = "Restores the initial Danish family showcase dataset in demo.db. Forbidden for Owner workspace.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        if (!_tenantContext.IsDemo)
        {
            await HttpContext.Response.SendResultAsync(TypedResults.Forbid());
            return;
        }

        await _demoDataSeeder.ResetAndSeedAsync(ct);

        await HttpContext.Response.SendResultAsync(TypedResults.Ok(new ResetDemoDataResponse
        {
            Message = "Demo sandbox reset to initial Danish family showcase state.",
            ResetAt = DateTime.UtcNow
        }));
    }
}
