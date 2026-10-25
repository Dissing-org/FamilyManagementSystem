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
            await EnsureTableExistsAsync(connection, logger, "Freezers", FreezersDdl, ct);
            await EnsureTableExistsAsync(connection, logger, "FreezerCatalogItems", FreezerCatalogItemsDdl, ct);
            await EnsureCatalogNormalizedNameColumnAsync(connection, logger, ct);
            await SeedCatalogFromFreezerItemsIfEmptyAsync(connection, logger, ct);
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

    private const string FreezersDdl = """
        CREATE TABLE "Freezers" (
            "Id" TEXT NOT NULL CONSTRAINT "PK_Freezers" PRIMARY KEY,
            "Name" TEXT NOT NULL,
            "Location" TEXT NULL,
            "DisplayOrder" INTEGER NOT NULL,
            "CreatedAt" TEXT NOT NULL
        );
        CREATE TABLE "FreezerCompartments" (
            "Id" TEXT NOT NULL CONSTRAINT "PK_FreezerCompartments" PRIMARY KEY,
            "FreezerId" TEXT NOT NULL,
            "Name" TEXT NOT NULL,
            "DisplayOrder" INTEGER NOT NULL,
            CONSTRAINT "FK_FreezerCompartments_Freezers_FreezerId" FOREIGN KEY ("FreezerId") REFERENCES "Freezers" ("Id") ON DELETE CASCADE
        );
        CREATE INDEX "IX_FreezerCompartments_FreezerId" ON "FreezerCompartments" ("FreezerId");
        CREATE TABLE "FreezerItems" (
            "Id" TEXT NOT NULL CONSTRAINT "PK_FreezerItems" PRIMARY KEY,
            "CompartmentId" TEXT NOT NULL,
            "Name" TEXT NOT NULL,
            "Category" TEXT NOT NULL,
            "Quantity" TEXT NOT NULL,
            "Unit" TEXT NOT NULL,
            "FreezeDate" TEXT NOT NULL,
            "ExpiryDate" TEXT NULL,
            "MinimumQuantity" TEXT NULL,
            "Notes" TEXT NULL,
            "CreatedAt" TEXT NOT NULL,
            "UpdatedAt" TEXT NULL
        );
        CREATE INDEX "IX_FreezerItems_CompartmentId" ON "FreezerItems" ("CompartmentId");
        CREATE INDEX "IX_FreezerItems_Category" ON "FreezerItems" ("Category");
        CREATE INDEX "IX_FreezerItems_ExpiryDate" ON "FreezerItems" ("ExpiryDate");
        CREATE TABLE "FreezerTransactions" (
            "Id" TEXT NOT NULL CONSTRAINT "PK_FreezerTransactions" PRIMARY KEY,
            "ItemId" TEXT NOT NULL,
            "QuantityChanged" TEXT NOT NULL,
            "RemainingQuantity" TEXT NOT NULL,
            "Reason" TEXT NOT NULL,
            "Notes" TEXT NULL,
            "Timestamp" TEXT NOT NULL
        );
        CREATE INDEX "IX_FreezerTransactions_ItemId" ON "FreezerTransactions" ("ItemId");
        CREATE INDEX "IX_FreezerTransactions_Timestamp" ON "FreezerTransactions" ("Timestamp");
        """;

    private const string FreezerCatalogItemsDdl = """
        CREATE TABLE "FreezerCatalogItems" (
            "Id" TEXT NOT NULL CONSTRAINT "PK_FreezerCatalogItems" PRIMARY KEY,
            "Name" TEXT NOT NULL,
            "NormalizedName" TEXT NOT NULL DEFAULT '',
            "DefaultCategory" TEXT NOT NULL,
            "DefaultUnit" TEXT NOT NULL,
            "DefaultMinimumQuantity" TEXT NULL,
            "CreatedAt" TEXT NOT NULL,
            "LastUsedAt" TEXT NOT NULL
        );
        CREATE INDEX "IX_FreezerCatalogItems_Name" ON "FreezerCatalogItems" ("Name");
        CREATE UNIQUE INDEX "IX_FreezerCatalogItems_NormalizedName" ON "FreezerCatalogItems" ("NormalizedName");
        """;

    private static async Task EnsureCatalogNormalizedNameColumnAsync(
        System.Data.Common.DbConnection connection,
        ILogger logger,
        CancellationToken ct)
    {
        var catalogColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        await using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "PRAGMA table_info(\"FreezerCatalogItems\");";
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                catalogColumns.Add(reader.GetString(1));
            }
        }

        if (catalogColumns.Contains("NormalizedName"))
        {
            return;
        }

        await using (var alterCmd = connection.CreateCommand())
        {
            alterCmd.CommandText = "ALTER TABLE \"FreezerCatalogItems\" ADD COLUMN \"NormalizedName\" TEXT NOT NULL DEFAULT '';";
            await alterCmd.ExecuteNonQueryAsync(ct);
        }

        var existingRows = new List<(string Id, string Name)>();
        await using (var selectCmd = connection.CreateCommand())
        {
            selectCmd.CommandText = "SELECT \"Id\", \"Name\" FROM \"FreezerCatalogItems\";";
            await using var reader = await selectCmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                existingRows.Add((reader.GetString(0), reader.GetString(1)));
            }
        }

        var seenNormalized = new HashSet<string>(StringComparer.Ordinal);
        foreach (var row in existingRows)
        {
            var normalized = row.Name.Trim().ToUpperInvariant();
            if (!seenNormalized.Add(normalized))
            {
                await using var deleteCmd = connection.CreateCommand();
                deleteCmd.CommandText = "DELETE FROM \"FreezerCatalogItems\" WHERE \"Id\" = @id;";
                var pDelId = deleteCmd.CreateParameter(); pDelId.ParameterName = "@id"; pDelId.Value = row.Id; deleteCmd.Parameters.Add(pDelId);
                await deleteCmd.ExecuteNonQueryAsync(ct);
                continue;
            }

            await using var updateCmd = connection.CreateCommand();
            updateCmd.CommandText = "UPDATE \"FreezerCatalogItems\" SET \"NormalizedName\" = @normalized WHERE \"Id\" = @id;";
            var pNorm = updateCmd.CreateParameter(); pNorm.ParameterName = "@normalized"; pNorm.Value = normalized; updateCmd.Parameters.Add(pNorm);
            var pId = updateCmd.CreateParameter(); pId.ParameterName = "@id"; pId.Value = row.Id; updateCmd.Parameters.Add(pId);
            await updateCmd.ExecuteNonQueryAsync(ct);
        }

        await using (var indexCmd = connection.CreateCommand())
        {
            indexCmd.CommandText = """
                DROP INDEX IF EXISTS "IX_FreezerCatalogItems_Name";
                CREATE INDEX IF NOT EXISTS "IX_FreezerCatalogItems_Name" ON "FreezerCatalogItems" ("Name");
                CREATE UNIQUE INDEX IF NOT EXISTS "IX_FreezerCatalogItems_NormalizedName" ON "FreezerCatalogItems" ("NormalizedName");
                """;
            await indexCmd.ExecuteNonQueryAsync(ct);
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Migrated FreezerCatalogItems table to include NormalizedName column and unique index.");
        }
    }

    private static async Task SeedCatalogFromFreezerItemsIfEmptyAsync(
        System.Data.Common.DbConnection connection,
        ILogger logger,
        CancellationToken ct)
    {
        await using var countCmd = connection.CreateCommand();
        countCmd.CommandText = "SELECT COUNT(*) FROM \"FreezerCatalogItems\";";
        var catalogCount = Convert.ToInt32(await countCmd.ExecuteScalarAsync(ct));
        if (catalogCount > 0)
        {
            return;
        }

        await using var itemCountCmd = connection.CreateCommand();
        itemCountCmd.CommandText = "SELECT COUNT(*) FROM \"FreezerItems\";";
        var itemCount = Convert.ToInt32(await itemCountCmd.ExecuteScalarAsync(ct));
        if (itemCount == 0)
        {
            return;
        }

        await using var selectCmd = connection.CreateCommand();
        selectCmd.CommandText = """
            SELECT "Name", "Category", "Unit", "MinimumQuantity", MAX("CreatedAt") as "LatestCreated"
            FROM "FreezerItems"
            GROUP BY "Name";
            """;

        var itemsByNormalizedName = new Dictionary<string, (string Name, string NormalizedName, string Category, string Unit, string? MinQty, string CreatedAt)>(StringComparer.Ordinal);
        await using (var reader = await selectCmd.ExecuteReaderAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                var rawName = reader.GetString(0);
                if (string.IsNullOrWhiteSpace(rawName))
                {
                    continue;
                }

                var trimmedName = rawName.Trim();
                var normalized = trimmedName.ToUpperInvariant();
                var category = reader.GetString(1);
                var unit = reader.GetString(2);
                var minQty = reader.IsDBNull(3) ? null : reader.GetString(3);
                var createdAt = reader.GetString(4);

                if (!itemsByNormalizedName.ContainsKey(normalized))
                {
                    itemsByNormalizedName[normalized] = (trimmedName, normalized, category, unit, minQty, createdAt);
                }
            }
        }

        var nowStr = DateTime.UtcNow.ToString("O");
        await using var tx = await connection.BeginTransactionAsync(ct);
        foreach (var item in itemsByNormalizedName.Values)
        {
            await using var insertCmd = connection.CreateCommand();
            insertCmd.Transaction = tx;
            insertCmd.CommandText = """
                INSERT OR IGNORE INTO "FreezerCatalogItems" ("Id", "Name", "NormalizedName", "DefaultCategory", "DefaultUnit", "DefaultMinimumQuantity", "CreatedAt", "LastUsedAt")
                VALUES (@id, @name, @normalizedName, @category, @unit, @minQty, @createdAt, @lastUsedAt);
                """;
            var pId = insertCmd.CreateParameter(); pId.ParameterName = "@id"; pId.Value = Guid.NewGuid().ToString(); insertCmd.Parameters.Add(pId);
            var pName = insertCmd.CreateParameter(); pName.ParameterName = "@name"; pName.Value = item.Name; insertCmd.Parameters.Add(pName);
            var pNorm = insertCmd.CreateParameter(); pNorm.ParameterName = "@normalizedName"; pNorm.Value = item.NormalizedName; insertCmd.Parameters.Add(pNorm);
            var pCat = insertCmd.CreateParameter(); pCat.ParameterName = "@category"; pCat.Value = item.Category; insertCmd.Parameters.Add(pCat);
            var pUnit = insertCmd.CreateParameter(); pUnit.ParameterName = "@unit"; pUnit.Value = item.Unit; insertCmd.Parameters.Add(pUnit);
            var pMin = insertCmd.CreateParameter(); pMin.ParameterName = "@minQty"; pMin.Value = (object?)item.MinQty ?? DBNull.Value; insertCmd.Parameters.Add(pMin);
            var pCreated = insertCmd.CreateParameter(); pCreated.ParameterName = "@createdAt"; pCreated.Value = item.CreatedAt; insertCmd.Parameters.Add(pCreated);
            var pUsed = insertCmd.CreateParameter(); pUsed.ParameterName = "@lastUsedAt"; pUsed.Value = nowStr; insertCmd.Parameters.Add(pUsed);

            await insertCmd.ExecuteNonQueryAsync(ct);
        }

        await tx.CommitAsync(ct);

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation("Seeded {Count} items into FreezerCatalogItems from existing FreezerItems.", itemsByNormalizedName.Count);
        }
    }
}
