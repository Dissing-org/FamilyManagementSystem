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

    // Auto-migrate SQLite schema for databases created by older versions of the app
    try
    {
        var connection = db.Database.GetDbConnection();
        if (connection.State != System.Data.ConnectionState.Open)
        {
            connection.Open();
        }

        var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "PRAGMA table_info(\"Receipts\");";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                columns.Add(reader.GetString(1));
            }
        }

        var migrations = new List<string>();
        if (!columns.Contains("Category"))
        {
            migrations.Add("ALTER TABLE \"Receipts\" ADD COLUMN \"Category\" TEXT NOT NULL DEFAULT 'Other';");
        }

        // Receipts only record who they are from; amount and currency are intentionally not tracked.
        foreach (var legacyColumn in new[] { "Amount", "Currency" })
        {
            if (columns.Contains(legacyColumn))
            {
                migrations.Add($"ALTER TABLE \"Receipts\" DROP COLUMN \"{legacyColumn}\";");
            }
        }

        foreach (var sql in migrations)
        {
            using var migrationCmd = connection.CreateCommand();
            migrationCmd.CommandText = sql;
            migrationCmd.ExecuteNonQuery();
            app.Logger.LogInformation("Applied SQLite schema migration: {Sql}", sql);
        }

        // Ensure InsurancePolicies table exists for databases created by older versions of the app
        using (var tableCmd = connection.CreateCommand())
        {
            tableCmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='InsurancePolicies';";
            var tableCount = Convert.ToInt32(tableCmd.ExecuteScalar());
            if (tableCount == 0)
            {
                tableCmd.CommandText = """
                    CREATE TABLE "InsurancePolicies" (
                        "Id" TEXT NOT NULL CONSTRAINT "PK_InsurancePolicies" PRIMARY KEY,
                        "Insurer" TEXT NOT NULL,
                        "PolicyNumber" TEXT NULL,
                        "Category" TEXT NOT NULL,
                        "InsuredParty" TEXT NOT NULL,
                        "PremiumAmount" TEXT NOT NULL,
                        "PremiumCurrency" TEXT NOT NULL,
                        "Frequency" TEXT NOT NULL,
                        "StartDate" TEXT NOT NULL,
                        "RenewalDate" TEXT NULL,
                        "DeductibleAmount" TEXT NULL,
                        "DeductibleCurrency" TEXT NULL,
                        "Notes" TEXT NULL,
                        "Status" TEXT NOT NULL,
                        "CreatedAt" TEXT NOT NULL,
                        "UpdatedAt" TEXT NULL
                    );
                """;
                tableCmd.ExecuteNonQuery();
                app.Logger.LogInformation("Created missing InsurancePolicies table in SQLite database.");
            }
        }

        // Ensure Child Development tables exist
        using (var tableCmd = connection.CreateCommand())
        {
            tableCmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='ChildProfiles';";
            var tableCount = Convert.ToInt32(tableCmd.ExecuteScalar());
            if (tableCount == 0)
            {
                tableCmd.CommandText = """
                    CREATE TABLE "ChildProfiles" (
                        "Id" TEXT NOT NULL CONSTRAINT "PK_ChildProfiles" PRIMARY KEY,
                        "FirstName" TEXT NOT NULL,
                        "LastName" TEXT NULL,
                        "DateOfBirth" TEXT NOT NULL,
                        "Gender" TEXT NOT NULL,
                        "AvatarUrl" TEXT NULL,
                        "ClothesSize" TEXT NULL,
                        "ShoeSize" TEXT NULL,
                        "HatSize" TEXT NULL,
                        "DiaperSize" TEXT NULL,
                        "CreatedAt" TEXT NOT NULL,
                        "UpdatedAt" TEXT NULL
                    );
                    CREATE TABLE "GrowthMeasurements" (
                        "Id" TEXT NOT NULL CONSTRAINT "PK_GrowthMeasurements" PRIMARY KEY,
                        "ChildId" TEXT NOT NULL,
                        "RecordedDate" TEXT NOT NULL,
                        "HeightCm" TEXT NULL,
                        "WeightKg" TEXT NULL,
                        "HeadCircumferenceCm" TEXT NULL,
                        "Notes" TEXT NULL,
                        "CreatedAt" TEXT NOT NULL,
                        "UpdatedAt" TEXT NULL
                    );
                    CREATE TABLE "ChildMilestones" (
                        "Id" TEXT NOT NULL CONSTRAINT "PK_ChildMilestones" PRIMARY KEY,
                        "ChildId" TEXT NOT NULL,
                        "Title" TEXT NOT NULL,
                        "Description" TEXT NULL,
                        "Category" TEXT NOT NULL,
                        "Status" TEXT NOT NULL,
                        "ExpectedAgeMonths" INTEGER NULL,
                        "ExpectedWindowMaxMonths" INTEGER NULL,
                        "IsStandardGuideline" INTEGER NOT NULL,
                        "AchievedDate" TEXT NULL,
                        "AchievedAgeMonths" INTEGER NULL,
                        "Notes" TEXT NULL,
                        "PhotoUrl" TEXT NULL,
                        "CreatedAt" TEXT NOT NULL,
                        "UpdatedAt" TEXT NULL
                    );
                """;
                tableCmd.ExecuteNonQuery();
                app.Logger.LogInformation("Created Child Development tables in SQLite database.");
            }
        }

        // Ensure Vehicle Management tables exist
        using (var tableCmd = connection.CreateCommand())
        {
            tableCmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='Vehicles';";
            var tableCount = Convert.ToInt32(tableCmd.ExecuteScalar());
            if (tableCount == 0)
            {
                tableCmd.CommandText = """
                    CREATE TABLE "Vehicles" (
                        "Id" TEXT NOT NULL CONSTRAINT "PK_Vehicles" PRIMARY KEY,
                        "Make" TEXT NOT NULL,
                        "Model" TEXT NOT NULL,
                        "Year" INTEGER NOT NULL,
                        "LicensePlate" TEXT NOT NULL,
                        "Vin" TEXT NULL,
                        "FuelType" TEXT NOT NULL,
                        "CurrentMileageKm" INTEGER NOT NULL,
                        "ServiceIntervalKm" INTEGER NULL,
                        "ServiceIntervalMonths" INTEGER NULL,
                        "NextInspectionDate" TEXT NULL,
                        "CreatedAt" TEXT NOT NULL,
                        "UpdatedAt" TEXT NULL
                    );
                    CREATE INDEX "IX_Vehicles_LicensePlate" ON "Vehicles" ("LicensePlate");

                    CREATE TABLE "MileageLogs" (
                        "Id" TEXT NOT NULL CONSTRAINT "PK_MileageLogs" PRIMARY KEY,
                        "VehicleId" TEXT NOT NULL,
                        "RecordedDate" TEXT NOT NULL,
                        "MileageKm" INTEGER NOT NULL,
                        "Notes" TEXT NULL,
                        "CreatedAt" TEXT NOT NULL
                    );
                    CREATE INDEX "IX_MileageLogs_VehicleId" ON "MileageLogs" ("VehicleId");
                    CREATE INDEX "IX_MileageLogs_VehicleId_RecordedDate" ON "MileageLogs" ("VehicleId", "RecordedDate");

                    CREATE TABLE "VehicleServiceRecords" (
                        "Id" TEXT NOT NULL CONSTRAINT "PK_VehicleServiceRecords" PRIMARY KEY,
                        "VehicleId" TEXT NOT NULL,
                        "ServiceDate" TEXT NOT NULL,
                        "MileageKm" INTEGER NOT NULL,
                        "Type" TEXT NOT NULL,
                        "Title" TEXT NOT NULL,
                        "Workshop" TEXT NULL,
                        "Cost" TEXT NULL,
                        "Notes" TEXT NULL,
                        "ReceiptId" TEXT NULL,
                        "CreatedAt" TEXT NOT NULL
                    );
                    CREATE INDEX "IX_VehicleServiceRecords_VehicleId" ON "VehicleServiceRecords" ("VehicleId");
                    CREATE INDEX "IX_VehicleServiceRecords_VehicleId_ServiceDate" ON "VehicleServiceRecords" ("VehicleId", "ServiceDate");
                """;
                tableCmd.ExecuteNonQuery();
                app.Logger.LogInformation("Created Vehicle Management tables in SQLite database.");
            }
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
