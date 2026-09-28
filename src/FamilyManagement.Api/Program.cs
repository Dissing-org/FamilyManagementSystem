using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using FamilyManagement.Application;
using FamilyManagement.Infrastructure;
using FamilyManagement.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire Service Defaults (OpenTelemetry, HealthChecks, Discovery)
builder.AddServiceDefaults();

// Add Clean Architecture Layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// Add CORS for Web and Mobile clients
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add FastEndpoints & OpenAPI Swagger
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

// Auto-create / migrate SQLite schema on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReceiptDbContext>();
    var connStr = db.Database.GetConnectionString();
    if (!string.IsNullOrWhiteSpace(connStr))
    {
        var match = System.Text.RegularExpressions.Regex.Match(connStr, @"Data Source=([^;]+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        if (match.Success)
        {
            var dbPath = match.Groups[1].Value.Trim();
            var dir = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
    db.Database.EnsureCreated();
}

app.MapDefaultEndpoints();

app.UseCors();
app.UseDefaultExceptionHandler();
app.UseFastEndpoints();
app.UseSwaggerGen();

app.Run();

// Make Program accessible for WebApplicationFactory in integration tests
public partial class Program { }
