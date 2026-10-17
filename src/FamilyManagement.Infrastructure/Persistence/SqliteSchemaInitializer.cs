using System.Data;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FamilyManagement.Infrastructure.Persistence;

public static class SqliteSchemaInitializer
{
    public static async Task EnsureSchemaAndMigrateAsync(
        ReceiptDbContext db,
        ILogger logger,
        CancellationToken ct = default)
    {
        var connStr = db.Database.GetConnectionString();
        if (!string.IsNullOrWhiteSpace(connStr))
        {
            var match = Regex.Match(connStr, @"Data Source=([^;]+)", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(500));
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

        await db.Database.EnsureCreatedAsync(ct);

        try
        {
            var connection = db.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync(ct);
            }

            using (var pragmaCmd = connection.CreateCommand())
            {
                pragmaCmd.CommandText = "PRAGMA journal_mode = WAL; PRAGMA busy_timeout = 5000;";
                await pragmaCmd.ExecuteNonQueryAsync(ct);
            }

            var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "PRAGMA table_info(\"Receipts\");";
                await using var reader = await cmd.ExecuteReaderAsync(ct);
                while (await reader.ReadAsync(ct))
                {
                    columns.Add(reader.GetString(1));
                }
            }

            var migrations = new List<string>();
            if (!columns.Contains("Category"))
            {
                migrations.Add("ALTER TABLE \"Receipts\" ADD COLUMN \"Category\" TEXT NOT NULL DEFAULT 'Other';");
            }

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
                await migrationCmd.ExecuteNonQueryAsync(ct);
                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation("Applied SQLite schema migration: {Sql}", sql);
                }
            }

            await EnsureTableExistsAsync(connection, logger, "InsurancePolicies", InsurancePoliciesDdl, ct);
            await EnsureTableExistsAsync(connection, logger, "ChildProfiles", ChildDevelopmentDdl, ct);
            await EnsureTableExistsAsync(connection, logger, "Vehicles", VehiclesDdl, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "SQLite schema migration check failed or skipped.");
        }
    }

    private static async Task EnsureTableExistsAsync(
        System.Data.Common.DbConnection connection,
        ILogger logger,
        string tableName,
        string ddl,
        CancellationToken ct)
    {
        await using var tableCmd = connection.CreateCommand();
        tableCmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=@tableName;";
        var param = tableCmd.CreateParameter();
        param.ParameterName = "@tableName";
        param.Value = tableName;
        tableCmd.Parameters.Add(param);
        var tableCount = Convert.ToInt32(await tableCmd.ExecuteScalarAsync(ct));
        if (tableCount == 0)
        {
            tableCmd.Parameters.Clear();
            tableCmd.CommandText = ddl;
            await tableCmd.ExecuteNonQueryAsync(ct);
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Created missing {TableName} schema in SQLite database.", tableName);
            }
        }
    }

    private const string InsurancePoliciesDdl = """
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

    private const string ChildDevelopmentDdl = """
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

    private const string VehiclesDdl = """
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
            "CreatedAt" TEXT NOT NULL,
            CONSTRAINT "FK_MileageLogs_Vehicles_VehicleId" FOREIGN KEY ("VehicleId") REFERENCES "Vehicles" ("Id") ON DELETE CASCADE
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
            "CreatedAt" TEXT NOT NULL,
            CONSTRAINT "FK_VehicleServiceRecords_Vehicles_VehicleId" FOREIGN KEY ("VehicleId") REFERENCES "Vehicles" ("Id") ON DELETE CASCADE
        );
        CREATE INDEX "IX_VehicleServiceRecords_VehicleId" ON "VehicleServiceRecords" ("VehicleId");
        CREATE INDEX "IX_VehicleServiceRecords_VehicleId_ServiceDate" ON "VehicleServiceRecords" ("VehicleId", "ServiceDate");
        """;
}
