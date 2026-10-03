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

// Add CORS for Web and Mobile clients (supports credentialed cookies from Cloudflare Access)
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

    // Auto-migrate SQLite schema if columns are missing from existing database volumes
    try
    {
        var connection = db.Database.GetDbConnection();
        connection.Open();
        using var cmd = connection.CreateCommand();
        cmd.CommandText = "PRAGMA table_info(\"Receipts\");";
        using var reader = cmd.ExecuteReader();
        var hasCategory = false;
        while (reader.Read())
        {
            var colName = reader.GetString(1);
            if (string.Equals(colName, "Category", StringComparison.OrdinalIgnoreCase))
            {
                hasCategory = true;
                break;
            }
        }
        reader.Close();

        if (!hasCategory)
        {
            using var alterCmd = connection.CreateCommand();
            alterCmd.CommandText = "ALTER TABLE \"Receipts\" ADD COLUMN \"Category\" TEXT NOT NULL DEFAULT 'Other';";
            alterCmd.ExecuteNonQuery();
        }
    }
    catch (Exception ex)
    {
        app.Logger.LogWarning(ex, "SQLite schema migration check failed or skipped.");
    }
}

app.MapDefaultEndpoints();

app.UseCors();
app.UseDefaultExceptionHandler();
app.UseFastEndpoints();
app.UseSwaggerGen();

app.Run();

// Make Program accessible for WebApplicationFactory in integration tests
public partial class Program { }
