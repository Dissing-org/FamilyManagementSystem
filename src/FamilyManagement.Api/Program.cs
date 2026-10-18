using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Authentication;
using FamilyManagement.Api.Auth;
using FamilyManagement.Application;
using FamilyManagement.Application.Interfaces;
using FamilyManagement.Infrastructure;
using FamilyManagement.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.Configure<AuthOptions>(
    builder.Configuration.GetSection(AuthOptions.SectionName));

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantContext, HttpContextTenantContext>();

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = AuthOptions.HybridScheme;
        options.DefaultAuthenticateScheme = AuthOptions.HybridScheme;
        options.DefaultChallengeScheme = AuthOptions.HybridScheme;
        options.DefaultForbidScheme = AuthOptions.HybridScheme;
    })
    .AddScheme<AuthenticationSchemeOptions, HybridAuthenticationHandler>(AuthOptions.HybridScheme, _ => { })
    .AddCookie(AuthOptions.DemoCookieScheme, options =>
    {
        options.Cookie.Name = AuthOptions.AuthCookieName;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddFastEndpoints();
builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Family Management System - Receipts API";
        s.Version = "v1";
        s.Description = "API for uploading, archiving, and managing purchase receipts.";
    };
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReceiptDbContext>();
    await SqliteSchemaInitializer.EnsureSchemaAndMigrateAsync(db, app.Logger);

    var demoSeeder = scope.ServiceProvider.GetRequiredService<IDemoDataSeeder>();
    await demoSeeder.EnsureSeededAsync();
}

app.MapDefaultEndpoints();

app.UseCors();
app.UseDefaultExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints(c =>
{
    c.Serializer.Options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
app.UseSwaggerGen();

await app.RunAsync();

#pragma warning disable ASP0027
public partial class Program { }
#pragma warning restore ASP0027
