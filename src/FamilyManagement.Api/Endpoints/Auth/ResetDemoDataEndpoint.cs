using FastEndpoints;
using FamilyManagement.Application.Interfaces;

namespace FamilyManagement.Api.Endpoints.Auth;

/// <summary>
/// Response payload confirming that the demo database sandbox was reset.
/// </summary>
public class ResetDemoDataResponse
{
    public string Message { get; set; } = string.Empty;
    public DateTime ResetAt { get; set; }
}

/// <summary>
/// Endpoint that resets and reseeds the demo sandbox database.
/// </summary>
public class ResetDemoDataEndpoint : EndpointWithoutRequest<ResetDemoDataResponse>
{
    private readonly ITenantContext _tenantContext;
    private readonly IDemoDataSeeder _demoDataSeeder;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResetDemoDataEndpoint"/> class.
    /// </summary>
    /// <param name="tenantContext">The tenant context providing workspace identity.</param>
    /// <param name="demoDataSeeder">The service responsible for wiping and reseeding demo data.</param>
    public ResetDemoDataEndpoint(ITenantContext tenantContext, IDemoDataSeeder demoDataSeeder)
    {
        _tenantContext = tenantContext;
        _demoDataSeeder = demoDataSeeder;
    }

    /// <summary>
    /// Configures the route mapping and OpenAPI specification for the endpoint.
    /// </summary>
    public override void Configure()
    {
        Post("/api/demo/reset");
        Summary(s =>
        {
            s.Summary = "Reset the demo sandbox database and receipt files";
            s.Description = "Restores the initial Danish family showcase dataset in demo.db. Forbidden for Owner workspace.";
        });
    }

    /// <summary>
    /// Handles the request to reset demo data, ensuring the caller is in the demo workspace.
    /// </summary>
    /// <param name="ct">Cancellation token for the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
